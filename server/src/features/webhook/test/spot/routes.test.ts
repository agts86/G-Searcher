import { describe, expect, it, vi } from 'vitest';
import { createHmac } from 'node:crypto';
import { createSpotRouter } from '../../src/spot/routes.js';
import { SpotService } from '../../src/spot/service.js';
import type { SpotReplyService } from '../../src/spot/reply.service.js';
import type { WebhookRepository } from '../../src/spot/repository.js';

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

function setup(): { app: ReturnType<typeof createSpotRouter>; repo: WebhookRepository } {
  const spotReplyService = {
    processEvent: vi.fn().mockResolvedValue(null),
  } as unknown as SpotReplyService;
  const repo: WebhookRepository = {
    createGourmetLocationLog: vi.fn().mockResolvedValue(undefined),
    createGourmetWordLog: vi.fn().mockResolvedValue(undefined),
    createJobLog: vi.fn().mockResolvedValue(undefined),
  };
  const service = new SpotService(spotReplyService, repo);
  const app = createSpotRouter(service, CHANNEL_SECRET, true);
  return { app, repo };
}

describe('POST /spot', () => {
  it('署名が正しければ同期処理し200とmeta配列を返す', async () => {
    const { app } = setup();

    const res = await app.request('/spot?genreCode=0101', buildRequestInit({ destination: 'U1', events: [] }));

    expect(res.status).toBe(200);
    const body = await res.json();
    expect(body).toEqual([]);
  });

  it('署名が無ければ401を返す', async () => {
    const { app } = setup();

    const res = await app.request('/spot', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ destination: 'U1', events: [] }),
    });

    expect(res.status).toBe(401);
  });
});
