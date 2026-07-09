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

const PersistedMetaSchema = z.union([
  z.object({ id: z.string(), lat: z.number(), lng: z.number(), createdAt: z.string(), updatedAt: z.string() }),
  z.object({ id: z.string(), text: z.string().nullable(), createdAt: z.string(), updatedAt: z.string() }),
]);

const LocalEventResultSchema = z.object({
  reply: z.object({ replyToken: z.string(), messages: z.array(z.unknown()) }),
  meta: PersistedMetaSchema,
  isReplySucceeded: z.boolean(),
});

export const LocalReplyResponseSchema = z.array(LocalEventResultSchema).openapi('LocalReplyResponse');
