import { OpenAPIHono } from '@hono/zod-openapi';
import { swaggerUI } from '@hono/swagger-ui';
import { createAuthRouter, AuthService, type AuthServiceConfig } from '@api/features-auth';
import { createManagedRouter, ManagedService } from '@api/features-managed';
import { createSpotRouter, SpotService, SpotReplyService, ParkingReplyService, createParkingRouter } from '@api/features-webhook';
import {
  getPrismaClient,
  PrismaAuthRepository,
  PrismaManagedRepository,
  PrismaWebhookRepository,
  HttpAdapter,
  YolpClientImpl,
  LineReplyClientImpl,
  ParkingClientImpl,
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
  // ローカルでcurl等を使いLINE実機なしに疎通確認したい場合のみ明示的に無効化する。
  // NODE_ENVに連動させるとテスト実行時(NODE_ENV=test)も自動でfalseになり、
  // 既存の401検証テスト（署名なしリクエストの拒否）が壊れるため専用フラグにする。
  const verifyLineSignature = process.env.DISABLE_LINE_SIGNATURE_VERIFICATION !== 'true';
  const httpAdapter = new HttpAdapter();
  const spotSearchClient = new YolpClientImpl(httpAdapter, requireEnv('YAHOO_APP_ID'));
  const lineReplyClient = new LineReplyClientImpl(requireEnv('LINE_CHANNEL_ACCESS_TOKEN'));
  const spotReplyService = new SpotReplyService(spotSearchClient, lineReplyClient, skipLineApiCall);
  const webhookRepository = new PrismaWebhookRepository(prisma);
  const spotService = new SpotService(spotReplyService, webhookRepository);
  const spotRouter = createSpotRouter(spotService, requireEnv('LINE_CHANNEL_SECRET'), verifyLineSignature);

  // 全国バイク駐車場案内(jmpsa.or.jp)検索版。DB永続化は行わずLINE返信のみ同期的に行う。
  // スポット検索とは別のLINEチャンネルで運用するためChannel Secret / Access Tokenを分離する。
  const parkingClient = new ParkingClientImpl(httpAdapter);
  const parkingLineReplyClient = new LineReplyClientImpl(requireEnv('BIKE_PARKING_LINE_CHANNEL_ACCESS_TOKEN'));
  const parkingReplyService = new ParkingReplyService(parkingClient, parkingLineReplyClient, skipLineApiCall);
  const parkingRouter = createParkingRouter(
    parkingReplyService,
    requireEnv('BIKE_PARKING_LINE_CHANNEL_SECRET'),
    verifyLineSignature,
  );

  const app = new OpenAPIHono();
  app.get('/health', (c) => c.text('ok'));
  app.route('/api/v1/auth', authRouter);
  app.route('/api/v1/managed', managedRouter);
  app.route('/api/v1/webhook', spotRouter);
  app.route('/api/v1/webhook', parkingRouter);

  // 既存.NET側 Program.cs の `if (app.Environment.IsDevelopment())` と同じ考え方。
  // /doc・/ui は本番でAPI仕様を外部に露出させないため、本番では登録しない。
  if (process.env.NODE_ENV !== 'production') {
    app.doc('/doc', {
      openapi: '3.1.0',
      info: { title: 'api (Auth / Managed)', version: '0.1.0' },
    });
    app.get('/ui', swaggerUI({ url: '/doc' }));
  }

  return app;
}
