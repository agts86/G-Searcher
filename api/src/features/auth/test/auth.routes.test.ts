import { describe, expect, it } from 'vitest';
import { createAuthRouter } from '../src/auth.routes.js';
import { AuthService } from '../src/auth.service.js';
import { InMemoryAuthRepository } from './support/in-memory-auth-repository.js';

const config = {
  adminUserName: 'admin',
  adminPassword: 'admin',
  jwt: {
    secret: 'test-jwt-secret-at-least-32-characters-long',
    issuer: 'LineWebHookAPI',
    audience: 'LineWebHookAdmin',
  },
  accessTokenExpiresInSeconds: 900,
  refreshTokenExpiresInSeconds: 604800,
};

function setup(cookieSecure = true): { app: ReturnType<typeof createAuthRouter> } {
  const repo = new InMemoryAuthRepository();
  const service = new AuthService(repo, config);
  const app = createAuthRouter(service, config.jwt, cookieSecure);
  return { app };
}

function getSetCookies(res: Response): string[] {
  // Response.headers.getSetCookie() は undici/Node18+ で複数Set-Cookieを配列取得できる
  return res.headers.getSetCookie ? res.headers.getSetCookie() : [res.headers.get('set-cookie') ?? ''];
}

describe('POST /login', () => {
  it('成功時にuserNameとexpiresAtを返し、両Cookieを設定する', async () => {
    const { app } = setup();

    const res = await app.request('/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ userName: 'admin', password: 'admin' }),
    });

    expect(res.status).toBe(200);
    const body = await res.json();
    expect(body.userName).toBe('admin');
    expect(body.expiresAt).toBeTruthy();

    const cookies = getSetCookies(res).join(';');
    expect(cookies).toContain('linewebhook_auth');
    expect(cookies).toContain('linewebhook_refresh');
  });

  it('cookieSecure=falseならSecure属性の無いCookieを発行する（HTTPのSwagger UIから試せるように）', async () => {
    const { app } = setup(false);

    const res = await app.request('/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ userName: 'admin', password: 'admin' }),
    });

    const cookies = getSetCookies(res).join(';');
    expect(cookies).not.toContain('Secure');
    expect(cookies).toContain('HttpOnly');
  });

  it('失敗時は401を返しCookieを設定しない', async () => {
    const { app } = setup();

    const res = await app.request('/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ userName: 'admin', password: 'wrong' }),
    });

    expect(res.status).toBe(401);
    expect(res.headers.get('set-cookie')).toBeFalsy();
  });
});

describe('POST /refresh', () => {
  it('有効なリフレッシュCookieでトークンをローテーションする', async () => {
    const { app } = setup();
    const loginRes = await app.request('/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ userName: 'admin', password: 'admin' }),
    });
    const refreshCookie = getSetCookies(loginRes).find((c) => c.startsWith('linewebhook_refresh'));
    const refreshTokenValue = refreshCookie?.split(';')[0];

    const res = await app.request('/refresh', {
      method: 'POST',
      headers: { Cookie: refreshTokenValue ?? '' },
    });

    expect(res.status).toBe(200);
    const body = await res.json();
    expect(body.userName).toBe('admin');
  });

  it('リフレッシュCookieが無ければ401を返す', async () => {
    const { app } = setup();

    const res = await app.request('/refresh', { method: 'POST' });

    expect(res.status).toBe(401);
  });
});

describe('POST /logout', () => {
  it('常に204を返し、両Cookieを削除する', async () => {
    const { app } = setup();

    const res = await app.request('/logout', { method: 'POST' });

    expect(res.status).toBe(204);
    const cookies = getSetCookies(res).join(';');
    expect(cookies).toContain('linewebhook_auth');
    expect(cookies).toContain('linewebhook_refresh');
  });
});

describe('GET /me', () => {
  it('有効なアクセストークンがあれば200とuserNameを返す', async () => {
    const { app } = setup();
    const loginRes = await app.request('/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ userName: 'admin', password: 'admin' }),
    });
    const authCookie = getSetCookies(loginRes).find((c) => c.startsWith('linewebhook_auth'));
    const authCookieValue = authCookie?.split(';')[0];

    const res = await app.request('/me', {
      headers: { Cookie: authCookieValue ?? '' },
    });

    expect(res.status).toBe(200);
    const body = await res.json();
    expect(body.userName).toBe('admin');
  });

  it('アクセストークンが無ければ401を返す', async () => {
    const { app } = setup();

    const res = await app.request('/me');

    expect(res.status).toBe(401);
  });
});
