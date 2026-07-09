import { OpenAPIHono } from '@hono/zod-openapi';
import { serveStatic } from '@hono/node-server/serve-static';
import { swaggerUI } from '@hono/swagger-ui';
import { createAuthRouter, AuthService, type AuthServiceConfig } from '@api/features-auth';
import { createManagedRouter, ManagedService } from '@api/features-managed';
import { createWebhookRouter, WebhookService, LineReplyService } from '@api/features-webhook';
import {
  getPrismaClient,
  PrismaAuthRepository,
  PrismaManagedRepository,
  PrismaWebhookRepository,
  HttpAdapter,
  YolpClientImpl,
  LineReplyClientImpl,
} from '@api/infrastructure';

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

  // 既存.NET側 LineReplyService.PostLocalAsync の `if (Env.IsDevelopment()) return;`
  // と同じ考え方。開発環境ではLINEへの実際の返信APIコールをスキップする。
  const skipLineApiCall = process.env.NODE_ENV !== 'production';
  const httpAdapter = new HttpAdapter();
  const yolpClient = new YolpClientImpl(httpAdapter, requireEnv('YAHOO_APP_ID'));
  const lineReplyClient = new LineReplyClientImpl(requireEnv('LINE_CHANNEL_ACCESS_TOKEN'));
  const lineReplyService = new LineReplyService(yolpClient, lineReplyClient, skipLineApiCall);
  const webhookRepository = new PrismaWebhookRepository(prisma);
  const webhookService = new WebhookService(lineReplyService, webhookRepository);
  const webhookRouter = createWebhookRouter(webhookService, requireEnv('LINE_CHANNEL_SECRET'));

  const app = new OpenAPIHono();
  app.get('/health', (c) => c.text('ok'));
  app.route('/api/v1/auth', authRouter);
  app.route('/api/v1/managed', managedRouter);
  app.route('/api/v1/webhook', webhookRouter);

  // 既存.NET側 Program.cs の `if (app.Environment.IsDevelopment())` と同じ考え方。
  // /doc・/ui は本番でAPI仕様を外部に露出させないため、本番では登録しない。
  if (process.env.NODE_ENV !== 'production') {
    app.doc('/doc', {
      openapi: '3.1.0',
      info: { title: 'api (Auth / Managed)', version: '0.1.0' },
    });
    app.get('/ui', swaggerUI({ url: '/doc' }));
  }

  // 既存.NET側 `UseDefaultFiles()+UseStaticFiles()` 相当。SPA fallback（未知パスをindex.htmlへ）は
  // .NET側にも実装されていないため、ここでも同じ粒度（ディレクトリ配下のindex.html解決のみ）に留める。
  app.use(
    '*',
    serveStatic({
      root: './wwwroot',
      rewriteRequestPath: (path) => (path.endsWith('/') ? `${path}index.html` : path),
    }),
  );

  return app;
}
