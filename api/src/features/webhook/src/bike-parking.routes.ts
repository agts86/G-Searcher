import { OpenAPIHono, createRoute } from '@hono/zod-openapi';
import type { webhook } from '@line/bot-sdk';
import { createLineSignatureGuard } from './line-signature-guard.js';
import type { BikeParkingReplyService } from './bike-parking-reply.service.js';
import { WebhookRequestBodySchema, BikeParkingReplyResponseSchema } from './webhook.dto.js';

const tags = ['Webhook'];

const parkingRoute = createRoute({
  tags,
  method: 'post',
  path: '/parking',
  request: {
    body: { content: { 'application/json': { schema: WebhookRequestBodySchema } } },
  },
  responses: {
    200: {
      description: 'jmpsa.or.jp（全国バイク駐車場案内）検索・LINE返信まで同期的に完了し、送信した返信内容・返信成否の一覧を返す',
      content: { 'application/json': { schema: BikeParkingReplyResponseSchema } },
    },
  },
});

function toCallbackRequest(body: { destination: string; events: unknown[] }): webhook.CallbackRequest {
  return body as unknown as webhook.CallbackRequest;
}

/**
 * /parking を実装するOpenAPIHonoルーター。ホスト側で /api/v1/webhook にマウントする。
 * DB永続化・非同期ジョブキューは持たず、既存 /local と同じLINE署名検証のみを適用する。
 */
export function createBikeParkingRouter(
  service: BikeParkingReplyService,
  channelSecret: string,
  verifySignature = true,
): OpenAPIHono {
  const app = new OpenAPIHono();
  app.use('*', createLineSignatureGuard(channelSecret, verifySignature));

  app.openapi(parkingRoute, async (c) => {
    const body = c.req.valid('json');
    const results = await service.processEvents(toCallbackRequest(body));
    return c.json(results, 200);
  });

  return app;
}
