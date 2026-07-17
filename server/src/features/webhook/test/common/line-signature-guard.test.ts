import { describe, expect, it } from 'vitest';
import { Hono } from 'hono';
import { createHmac } from 'node:crypto';
import { createLineSignatureGuard } from '../../src/line-signature-guard.js';

const CHANNEL_SECRET = 'test-channel-secret';

function signBody(body: string): string {
  return createHmac('sha256', CHANNEL_SECRET).update(body).digest('base64');
}

function buildApp(verifySignature = true): Hono {
  const app = new Hono();
  app.use('*', createLineSignatureGuard(CHANNEL_SECRET, verifySignature));
  app.post('/webhook', async (c) => {
    const body = await c.req.json<Record<string, unknown>>();
    return c.json({ received: body });
  });
  return app;
}

describe('createLineSignatureGuard', () => {
  it('正しい署名なら通過し、後続ハンドラーがJSONボディを読める', async () => {
    const app = buildApp();
    const body = JSON.stringify({ destination: 'U123', events: [] });

    const res = await app.request('/webhook', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', 'x-line-signature': signBody(body) },
      body,
    });

    expect(res.status).toBe(200);
    const json = await res.json();
    expect(json.received).toEqual({ destination: 'U123', events: [] });
  });

  it('x-line-signatureヘッダーが無ければ401を返す', async () => {
    const app = buildApp();

    const res = await app.request('/webhook', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: '{}',
    });

    expect(res.status).toBe(401);
  });

  it('署名が不正なら401を返す', async () => {
    const app = buildApp();

    const res = await app.request('/webhook', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', 'x-line-signature': 'invalid-signature' },
      body: '{}',
    });

    expect(res.status).toBe(401);
  });

  it('verifySignature=falseなら署名を検証せず通過させる（ローカル開発用）', async () => {
    const app = buildApp(false);

    const res = await app.request('/webhook', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: '{"destination":"U123","events":[]}',
    });

    expect(res.status).toBe(200);
  });
});
