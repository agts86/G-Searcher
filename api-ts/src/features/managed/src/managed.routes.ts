import { OpenAPIHono, createRoute } from '@hono/zod-openapi';
import { createAuthGuard, formatAsJstIsoString, type JwtConfig } from '@api-ts/shared';
import type { ManagedService } from './managed.service.js';
import {
  MessageTypeParamSchema,
  GourmetLogsResponseSchema,
  ErrorLogsResponseSchema,
  JobLogsResponseSchema,
} from './managed.dto.js';

const tags = ['Managed'];

const gourmetLogsRoute = createRoute({
  tags,
  method: 'get',
  path: '/gourmet/{messageType}',
  request: { params: MessageTypeParamSchema },
  responses: {
    200: {
      description: 'ロケーション/ワードのログ一覧（createdAt降順）',
      content: { 'application/json': { schema: GourmetLogsResponseSchema } },
    },
  },
});

const errorLogRoute = createRoute({
  tags,
  method: 'get',
  path: '/error-log',
  responses: {
    200: {
      description: 'エラーログ一覧（createdAt降順）',
      content: { 'application/json': { schema: ErrorLogsResponseSchema } },
    },
  },
});

const jobLogRoute = createRoute({
  tags,
  method: 'get',
  path: '/job-log',
  responses: {
    200: {
      description: 'ジョブログ一覧（createdAt降順）',
      content: { 'application/json': { schema: JobLogsResponseSchema } },
    },
  },
});

function withJstTimestamps<T extends { createdAt: Date; updatedAt: Date }>(
  row: T,
): Omit<T, 'createdAt' | 'updatedAt'> & { createdAt: string; updatedAt: string } {
  return { ...row, createdAt: formatAsJstIsoString(row.createdAt), updatedAt: formatAsJstIsoString(row.updatedAt) };
}

/** gourmet/error-log/job-log を実装するOpenAPIHonoルーター。ホスト側で /api/v1/managed にマウントする */
export function createManagedRouter(service: ManagedService, jwtConfig: JwtConfig): OpenAPIHono {
  const app = new OpenAPIHono();

  app.use('*', createAuthGuard(jwtConfig));

  app.openapi(gourmetLogsRoute, async (c) => {
    const { messageType } = c.req.valid('param');
    if (messageType === 'location') {
      const logs = await service.getGourmetLogs('location');
      return c.json(logs.map(withJstTimestamps), 200);
    }
    const logs = await service.getGourmetLogs('text');
    return c.json(logs.map(withJstTimestamps), 200);
  });

  app.openapi(errorLogRoute, async (c) => {
    const logs = await service.getErrorLogs();
    return c.json(logs.map(withJstTimestamps), 200);
  });

  app.openapi(jobLogRoute, async (c) => {
    const logs = await service.getJobLogs();
    return c.json(logs.map(withJstTimestamps), 200);
  });

  return app;
}
