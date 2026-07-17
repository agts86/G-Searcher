import { z } from '@hono/zod-openapi';

const ParkingEventResultSchema = z.object({
  reply: z.object({ replyToken: z.string(), messages: z.array(z.unknown()) }),
  isReplySucceeded: z.boolean(),
});

export const ParkingReplyResponseSchema = z.array(ParkingEventResultSchema).openapi('ParkingReplyResponse');
