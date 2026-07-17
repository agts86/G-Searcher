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
