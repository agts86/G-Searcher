import { describe, expect, it, beforeAll } from 'vitest';
import { createHmac } from 'node:crypto';
import { createApp } from '../src/app.js';

function sign(secret: string, body: string): string {
  return createHmac('sha256', secret).update(body).digest('base64');
}

// createManagedRouter単体では401を返すテストが既にあるが、
// 実サーバーと同じ組み立て方（host/app.tsのcreateApp()、app.route()でマウント）で
// 認可ミドルウェアが実際に効くかをここで検証する。
beforeAll(() => {
  process.env.JWT_SECRET = 'test-jwt-secret-at-least-32-characters-long';
  process.env.ADMIN_USERNAME = 'admin';
  process.env.ADMIN_PASSWORD = 'admin';
  process.env.DATABASE_URL = 'postgresql://postgres:postgres@localhost:5432/postgres?schema=public&sslmode=disable';
  process.env.LINE_CHANNEL_SECRET = 'test-line-channel-secret';
  process.env.LINE_CHANNEL_ACCESS_TOKEN = 'test-line-channel-access-token';
  process.env.BIKE_PARKING_LINE_CHANNEL_SECRET = 'test-bike-parking-line-channel-secret';
  process.env.BIKE_PARKING_LINE_CHANNEL_ACCESS_TOKEN = 'test-bike-parking-line-channel-access-token';
  process.env.YAHOO_APP_ID = 'test-yahoo-app-id';
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

describe('createApp() 経由でマウントしたWebhookルート', () => {
  it('x-line-signatureヘッダーが無いまま POST /api/v1/webhook/local/accept を叩くと401を返す', async () => {
    const app = createApp();

    const res = await app.request('/api/v1/webhook/local/accept?genreCode=0301', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ destination: 'U123', events: [] }),
    });

    expect(res.status).toBe(401);
  });

  it('x-line-signatureヘッダーが無いまま POST /api/v1/webhook/parking を叩くと401を返す', async () => {
    const app = createApp();

    const res = await app.request('/api/v1/webhook/parking', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ destination: 'U123', events: [] }),
    });

    expect(res.status).toBe(401);
  });

  // webhookRouterとbikeParkingRouterを同じ/api/v1/webhookにapp.route()で二重マウントした際、
  // 先にマウントされた側のミドルウェア（LINE_CHANNEL_SECRET検証）が後からマウントされた
  // bikeParkingRouterのパスにも適用されてしまい、正しいBIKE_PARKING_LINE_CHANNEL_SECRETで
  // 署名しても401になる回帰バグがあった。単体ルーターのテストでは検出できないため、
  // createApp()経由（実際にマウントされる構成）で正しい署名なら通ることを検証する。
  it('BIKE_PARKING_LINE_CHANNEL_SECRETで正しく署名したPOST /api/v1/webhook/parkingは401にならない', async () => {
    const app = createApp();
    const body = JSON.stringify({ destination: 'U123', events: [] });

    const res = await app.request('/api/v1/webhook/parking', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', 'x-line-signature': sign('test-bike-parking-line-channel-secret', body) },
      body,
    });

    expect(res.status).not.toBe(401);
  });

  it('LINE_CHANNEL_SECRETで正しく署名したPOST /api/v1/webhook/local/acceptは401にならない', async () => {
    const app = createApp();
    const body = JSON.stringify({ destination: 'U123', events: [] });

    const res = await app.request('/api/v1/webhook/local/accept', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', 'x-line-signature': sign('test-line-channel-secret', body) },
      body,
    });

    expect(res.status).not.toBe(401);
  });
});
