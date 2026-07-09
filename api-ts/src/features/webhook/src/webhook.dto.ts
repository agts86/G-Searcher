import { z } from '@hono/zod-openapi';

// LINEプラットフォームからの署名済みリクエストのため、events内部の詳細な形は
// @line/bot-sdkのTypeScript型（webhook.WebhookEvent等）をサービス層で使い、
// ここでは大枠のみをZodで検証する。
export const WebhookRequestBodySchema = z
  .object({
    destination: z.string(),
    events: z.array(z.unknown()),
  })
  .openapi('WebhookRequestBody');

export const GenreCodeQuerySchema = z.object({
  genreCode: z.string().max(7).optional(),
});

export const AcceptResponseSchema = z
  .object({
    id: z.string(),
  })
  .openapi('AcceptResponse');

export const GourmetLogEntrySchema = z.union([
  z.object({ type: z.literal('location'), lat: z.number(), lng: z.number() }),
  z.object({ type: z.literal('word'), text: z.string().nullable() }),
]);
export const LocalReplyResponseSchema = z.array(GourmetLogEntrySchema).openapi('LocalReplyResponse');
