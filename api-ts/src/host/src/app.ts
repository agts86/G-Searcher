import { OpenAPIHono } from '@hono/zod-openapi';
import { swaggerUI } from '@hono/swagger-ui';
import { createAuthRouter, AuthService, type AuthServiceConfig } from '@api-ts/features-auth';
import { createManagedRouter, ManagedService } from '@api-ts/features-managed';
import { getPrismaClient, PrismaAuthRepository, PrismaManagedRepository } from '@api-ts/infrastructure';

function requireEnv(name: string): string {
  const value = process.env[name];
  if (!value) {
    throw new Error(`Missing required environment variable: ${name}`);
  }
  return value;
}

function buildAuthServiceConfig(): AuthServiceConfig {
  const jwtSecret = requireEnv('JWT_SECRET');
  if (jwtSecret.length < 32) {
    throw new Error('JWT_SECRET must be at least 32 characters.');
  }

  return {
    adminUserName: requireEnv('ADMIN_USERNAME'),
    adminPassword: requireEnv('ADMIN_PASSWORD'),
    jwt: {
      secret: jwtSecret,
      issuer: process.env.JWT_ISSUER ?? 'LineWebHookAPI',
      audience: process.env.JWT_AUDIENCE ?? 'LineWebHookAdmin',
    },
    accessTokenExpiresInSeconds: Number(process.env.ACCESS_TOKEN_EXPIRES_MINUTES ?? '15') * 60,
    refreshTokenExpiresInSeconds: Number(process.env.REFRESH_TOKEN_EXPIRES_DAYS ?? '7') * 86400,
  };
}

/**
 * Honoアプリを組み立てる（DI配線: Program.cs相当）。テストからも呼べるようexportする。
 * 戻り値型は意図的に明示しない: Honoの`.route()`は呼び出しごとに型を細分化するため、
 * 明示アノテーションと実際の推論結果が食い違う（Hono公式が推奨する書き方に従う）。
 */
// eslint-disable-next-line @typescript-eslint/explicit-function-return-type -- 理由は上のコメント参照
export function createApp() {
  const config = buildAuthServiceConfig();
  const prisma = getPrismaClient();
  // 本番以外（Swagger UIが見える環境と同じ条件）はSecure Cookieを外し、
  // HTTPのローカル開発環境でもSwagger UIの Try it out からログイン状態を維持できるようにする。
  const cookieSecure = process.env.NODE_ENV === 'production';

  const authRepository = new PrismaAuthRepository(prisma);
  const authService = new AuthService(authRepository, config);
  const authRouter = createAuthRouter(authService, config.jwt, cookieSecure);

  const managedRepository = new PrismaManagedRepository(prisma);
  const managedService = new ManagedService(managedRepository);
  const managedRouter = createManagedRouter(managedService, config.jwt);

  const app = new OpenAPIHono();
  app.get('/health', (c) => c.text('ok'));
  app.route('/api/v1/auth', authRouter);
  app.route('/api/v1/managed', managedRouter);

  // 既存.NET側 Program.cs の `if (app.Environment.IsDevelopment())` と同じ考え方。
  // /doc・/ui は本番でAPI仕様を外部に露出させないため、本番では登録しない。
  if (process.env.NODE_ENV !== 'production') {
    app.doc('/doc', {
      openapi: '3.1.0',
      info: { title: 'api-ts (Auth / Managed)', version: '0.1.0' },
    });
    app.get('/ui', swaggerUI({ url: '/doc' }));
  }

  return app;
}
