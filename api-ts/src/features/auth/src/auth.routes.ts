import { OpenAPIHono, createRoute } from '@hono/zod-openapi';
import type { Context } from 'hono';
import { getCookie, setCookie, deleteCookie } from 'hono/cookie';
import {
  AUTH_COOKIE_NAME,
  AUTH_REFRESH_COOKIE_NAME,
  buildAuthCookieOptions,
  verifyAccessToken,
  type JwtConfig,
} from '@api-ts/shared';
import { UnauthorizedError } from './auth.service.js';
import type { AuthService, LoginResult } from './auth.service.js';
import { LoginRequestSchema, LoginResponseSchema, MeResponseSchema, ErrorResponseSchema } from './auth.dto.js';

function setAuthCookies(c: Context, result: LoginResult): void {
  setCookie(c, AUTH_COOKIE_NAME, result.accessToken, buildAuthCookieOptions(result.accessTokenExpiresAt));
  setCookie(
    c,
    AUTH_REFRESH_COOKIE_NAME,
    result.refreshToken,
    buildAuthCookieOptions(result.refreshTokenExpiresAt),
  );
}

function loginResponseBody(result: LoginResult): { userName: string; expiresAt: string } {
  return { userName: result.userName, expiresAt: result.accessTokenExpiresAt.toISOString() };
}

const tags = ['Auth'];

const loginRoute = createRoute({
  tags,
  method: 'post',
  path: '/login',
  request: {
    body: { content: { 'application/json': { schema: LoginRequestSchema } } },
  },
  responses: {
    200: {
      description: 'ログイン成功。linewebhook_auth / linewebhook_refresh Cookieを発行する',
      content: { 'application/json': { schema: LoginResponseSchema } },
    },
    401: {
      description: 'ユーザー名またはパスワードが不正',
      content: { 'application/json': { schema: ErrorResponseSchema } },
    },
  },
});

const refreshRoute = createRoute({
  tags,
  method: 'post',
  path: '/refresh',
  responses: {
    200: {
      description: 'リフレッシュ成功。トークンをローテーションする',
      content: { 'application/json': { schema: LoginResponseSchema } },
    },
    401: {
      description: 'リフレッシュトークンが無効・期限切れ・未指定',
      content: { 'application/json': { schema: ErrorResponseSchema } },
    },
  },
});

const logoutRoute = createRoute({
  tags,
  method: 'post',
  path: '/logout',
  responses: {
    204: { description: 'ログアウト成功（常に成功する、冪等）' },
  },
});

const meRoute = createRoute({
  tags,
  method: 'get',
  path: '/me',
  responses: {
    200: {
      description: 'ログイン中のユーザー情報',
      content: { 'application/json': { schema: MeResponseSchema } },
    },
    401: {
      description: 'アクセストークンが無効・未指定',
      content: { 'application/json': { schema: ErrorResponseSchema } },
    },
  },
});

/**
 * login/refresh/logout/me を実装するOpenAPIHonoルーター。ホスト側で /api/v1/auth にマウントする。
 * eslint max-lines-per-function(50)を超過するが、`app.openapi()`の型推論を保つには
 * ハンドラーをこの関数内でservice/jwtConfigをクロージャ捕捉したまま定義する必要があり、
 * 外に切り出すと型安全性(c.req.valid()の型)を失う。行数警告よりも型安全性を優先する。
 */
// eslint-disable-next-line max-lines-per-function -- 理由は上のコメント参照
export function createAuthRouter(service: AuthService, jwtConfig: JwtConfig): OpenAPIHono {
  const app = new OpenAPIHono();

  app.openapi(loginRoute, async (c) => {
    const { userName, password } = c.req.valid('json');

    try {
      const result = await service.login(userName, password);
      setAuthCookies(c, result);
      return c.json(loginResponseBody(result), 200);
    } catch (err) {
      if (err instanceof UnauthorizedError) {
        return c.json({ message: err.message }, 401);
      }
      throw err;
    }
  });

  app.openapi(refreshRoute, async (c) => {
    const refreshToken = getCookie(c, AUTH_REFRESH_COOKIE_NAME);

    try {
      const result = await service.refresh(refreshToken);
      setAuthCookies(c, result);
      return c.json(loginResponseBody(result), 200);
    } catch (err) {
      if (err instanceof UnauthorizedError) {
        return c.json({ message: err.message }, 401);
      }
      throw err;
    }
  });

  app.openapi(logoutRoute, async (c) => {
    const refreshToken = getCookie(c, AUTH_REFRESH_COOKIE_NAME);
    await service.logout(refreshToken);

    deleteCookie(c, AUTH_COOKIE_NAME, { path: '/' });
    deleteCookie(c, AUTH_REFRESH_COOKIE_NAME, { path: '/' });
    return c.body(null, 204);
  });

  app.openapi(meRoute, async (c) => {
    const accessToken = getCookie(c, AUTH_COOKIE_NAME);
    if (!accessToken) {
      return c.json({ message: 'Unauthorized.' }, 401);
    }

    try {
      const { userName } = await verifyAccessToken(accessToken, jwtConfig);
      return c.json({ userName }, 200);
    } catch {
      return c.json({ message: 'Unauthorized.' }, 401);
    }
  });

  return app;
}
