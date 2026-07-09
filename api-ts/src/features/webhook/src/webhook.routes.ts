import { OpenAPIHono, createRoute } from '@hono/zod-openapi';
import type { webhook } from '@line/bot-sdk';
import { createLineSignatureGuard } from './line-signature-guard.js';
import { AsyncQueue } from './async-queue.js';
import type { WebhookService } from './webhook.service.js';
import type { LocalJob, LocalJobResult } from './webhook.types.js';
import { WebhookRequestBodySchema, GenreCodeQuerySchema, AcceptResponseSchema, LocalReplyResponseSchema } from './webhook.dto.js';

const tags = ['Webhook'];

const acceptRoute = createRoute({
  tags,
  method: 'post',
  path: '/local/accept',
  request: {
    query: GenreCodeQuerySchema,
    body: { content: { 'application/json': { schema: WebhookRequestBodySchema } } },
  },
  responses: {
    202: {
      description: 'ジョブをキューに登録した（DB書込・YOLP検索・LINE返信は非同期で後から実行される）',
      content: { 'application/json': { schema: AcceptResponseSchema } },
    },
  },
});

const localRoute = createRoute({
  tags,
  method: 'post',
  path: '/local',
  request: {
    query: GenreCodeQuerySchema,
    body: { content: { 'application/json': { schema: WebhookRequestBodySchema } } },
  },
  responses: {
    200: {
      description: 'YOLP検索・LINE返信・DB保存まで同期的に完了し、保存したログの一覧を返す',
      content: { 'application/json': { schema: LocalReplyResponseSchema } },
    },
  },
});

/**
 * このリクエストで受け取ったbodyはLINE署名検証済みだが、Zodスキーマではevents内部までは
 * 厳密に検証していない（LINEの複雑なイベントUnion型を再実装しない方針）ため、
 * webhook.CallbackRequestへの変換はここで明示的に行う。
 */
function toCallbackRequest(body: { destination: string; events: unknown[] }): webhook.CallbackRequest {
  return body as unknown as webhook.CallbackRequest;
}

/**
 * local/accept・local の2エンドポイントを実装するOpenAPIHonoルーター。
 * ホスト側で /api/v1/webhook にマウントする。
 * キューワーカー（ジョブ処理・結果永続化）はルーター生成時に起動する（既存.NET側の
 * BackgroundServiceに相当、プロセス常駐中ずっと動き続ける）。
 */
export function createWebhookRouter(service: WebhookService, channelSecret: string, verifySignature = true): OpenAPIHono {
  const app = new OpenAPIHono();
  app.use('*', createLineSignatureGuard(channelSecret, verifySignature));

  const jobQueue = new AsyncQueue<LocalJob>();
  const resultQueue = new AsyncQueue<LocalJobResult>();
  runJobWorker(jobQueue, resultQueue, service);
  runResultWorker(resultQueue, service);

  app.openapi(acceptRoute, (c) => {
    const body = c.req.valid('json');
    const { genreCode } = c.req.valid('query');
    const job = service.enqueueLocalJob(toCallbackRequest(body), genreCode);
    jobQueue.enqueue(job);
    return c.json({ id: job.id }, 202);
  });

  app.openapi(localRoute, async (c) => {
    const body = c.req.valid('json');
    const { genreCode } = c.req.valid('query');
    const metas = await service.processSync(toCallbackRequest(body), genreCode);
    return c.json(metas, 200);
  });

  return app;
}

function runJobWorker(jobQueue: AsyncQueue<LocalJob>, resultQueue: AsyncQueue<LocalJobResult>, service: WebhookService): void {
  const loop = async (): Promise<void> => {
    for (;;) {
      const job = await jobQueue.dequeue();
      const result = await service.processJob(job);
      resultQueue.enqueue(result);
    }
  };
  void loop();
}

function runResultWorker(resultQueue: AsyncQueue<LocalJobResult>, service: WebhookService): void {
  const loop = async (): Promise<void> => {
    for (;;) {
      const result = await resultQueue.dequeue();
      try {
        await service.persistJobResult(result);
      } catch {
        // 既存.NET側BackgroundServiceと同様、永続化失敗で常駐ループ自体を落とさない
      }
    }
  };
  void loop();
}
