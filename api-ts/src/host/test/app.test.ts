import { describe, expect, it, beforeAll } from 'vitest';
import { createApp } from '../src/app.js';

// createManagedRouter単体では401を返すテストが既にあるが、
// 実サーバーと同じ組み立て方（host/app.tsのcreateApp()、app.route()でマウント）で
// 認可ミドルウェアが実際に効くかをここで検証する。
beforeAll(() => {
  process.env.JWT_SECRET = 'test-jwt-secret-at-least-32-characters-long';
  process.env.ADMIN_USERNAME = 'admin';
  process.env.ADMIN_PASSWORD = 'admin';
  process.env.DATABASE_URL = 'postgresql://postgres:postgres@localhost:5432/postgres?schema=public&sslmode=disable';
});

describe('createApp() 経由でマウントしたManagedルート', () => {
  it('未認証で GET /api/v1/managed/error-log を叩くと401を返す', async () => {
    const app = createApp();

    const res = await app.request('/api/v1/managed/error-log');

    expect(res.status).toBe(401);
  });

  it('未認証で GET /api/v1/managed/job-log を叩くと401を返す', async () => {
    const app = createApp();

    const res = await app.request('/api/v1/managed/job-log');

    expect(res.status).toBe(401);
  });

  it('未認証で GET /api/v1/managed/gourmet/location を叩くと401を返す', async () => {
    const app = createApp();

    const res = await app.request('/api/v1/managed/gourmet/location');

    expect(res.status).toBe(401);
  });
});
