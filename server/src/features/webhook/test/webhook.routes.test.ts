import { describe, expect, it, vi } from 'vitest';
import { createHmac } from 'node:crypto';
import { createWebhookRouter } from '../src/webhook.routes.js';
import { WebhookService } from '../src/webhook.service.js';
import type { LineReplyService } from '../src/line-reply.service.js';
import type { WebhookRepository } from '../src/webhook.repository.js';

const CHANNEL_SECRET = 'test-channel-secret';

function signBody(body: string): string {
  return createHmac('sha256', CHANNEL_SECRET).update(body).digest('base64');
}

function buildRequestInit(body: object): RequestInit {
  const json = JSON.stringify(body);
  return {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', 'x-line-signature': signBody(json) },
    body: json,
  };
}

function setup(): { app: ReturnType<typeof createWebhookRouter>; repo: WebhookRepository } {
  const lineReplyService = {
    processEvent: vi.fn().mockResolvedValue(null),
  } as unknown as LineReplyService;
  const repo: WebhookRepository = {
    createGourmetLocationLog: vi.fn().mockResolvedValue(undefined),
    createGourmetWordLog: vi.fn().mockResolvedValue(undefined),
    createJobLog: vi.fn().mockResolvedValue(undefined),
  };
  const service = new WebhookService(lineReplyService, repo);
  const app = createWebhookRouter(service, CHANNEL_SECRET, true);
  return { app, repo };
}

describe('POST /local/accept', () => {
  it('署名が正しければ202とjob idを即座に返す（DB書込を待たない）', async () => {
    const { app } = setup();

    const res = await app.request('/local/accept?genreCode=0101', buildRequestInit({ destination: 'U1', events: [] }));

    expect(res.status).toBe(202);
    const body = await res.json();
    expect(body.id).toBeTruthy();
  });

  it('署名が無ければ401を返す', async () => {
    const { app } = setup();

    const res = await app.request('/local/accept', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ destination: 'U1', events: [] }),
    });

    expect(res.status).toBe(401);
  });
});

describe('POST /local', () => {
  it('署名が正しければ同期処理し200とmeta配列を返す', async () => {
    const { app } = setup();

    const res = await app.request('/local?genreCode=0101', buildRequestInit({ destination: 'U1', events: [] }));

    expect(res.status).toBe(200);
    const body = await res.json();
    expect(body).toEqual([]);
  });

  it('署名が無ければ401を返す', async () => {
    const { app } = setup();

    const res = await app.request('/local', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ destination: 'U1', events: [] }),
    });

    expect(res.status).toBe(401);
  });
});
