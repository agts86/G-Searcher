import { OpenAPIHono, createRoute } from '@hono/zod-openapi';
import type { webhook } from '@line/bot-sdk';
import { createLineSignatureGuard } from './line-signature-guard.js';
import type { WebhookService } from './webhook.service.js';
import { WebhookRequestBodySchema, GenreCodeQuerySchema, LocalReplyResponseSchema } from './webhook.dto.js';

const tags = ['Webhook'];

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
      description: 'YOLP検索・LINE返信・DB保存まで同期的に完了し、送信した返信内容・保存したログ・返信成否の一覧を返す',
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
 * /local を実装するOpenAPIHonoルーター。ホスト側で /api/v1/webhook にマウントする。
 * YOLP検索・LINE返信・DB保存までリクエスト内で同期的に完了する。
 */
export function createWebhookRouter(service: WebhookService, channelSecret: string, verifySignature = true): OpenAPIHono {
  const app = new OpenAPIHono();
  // '*'にすると、ホスト側で他のルーター（bikeParkingRouter等）と同じベースパスに
  // app.route()で並べてマウントした際、このミドルウェアが他ルーターのパスにも先に
  // 適用され誤ったchannelSecretで検証されてしまうため、自身が処理するパスに限定する。
  app.use('/local', createLineSignatureGuard(channelSecret, verifySignature));

  app.openapi(localRoute, async (c) => {
    const body = c.req.valid('json');
    const { genreCode } = c.req.valid('query');
    const results = await service.processSync(toCallbackRequest(body), genreCode);
    return c.json(results, 200);
  });

  return app;
}
