import { describe, expect, it, vi } from 'vitest';
import type { webhook } from '@line/bot-sdk';
import { WebhookService } from '../src/webhook.service.js';
import type { LineReplyService } from '../src/line-reply.service.js';
import type { WebhookRepository } from '../src/webhook.repository.js';
import type { LocalEventResult } from '../src/webhook.types.js';

function buildWebhookBody(events: webhook.Event[] = []): webhook.CallbackRequest {
  return { destination: 'U123', events };
}

function messageEvent(): webhook.Event {
  return {
    type: 'message',
    replyToken: 'token-1',
    message: { type: 'text', id: 'm1', text: 'ラーメン', quoteToken: 'q1' },
    timestamp: 0,
    mode: 'active',
    webhookEventId: 'we-1',
    deliveryContext: { isRedelivery: false },
  };
}

function wordMeta(text: string): { id: string; text: string; createdAt: string; updatedAt: string } {
  return { id: 'meta-1', text, createdAt: '2026-01-01T00:00:00.000+09:00', updatedAt: '2026-01-01T00:00:00.000+09:00' };
}

function buildService(
  processEventResult: LocalEventResult | null = {
    reply: { replyToken: 'token-1', messages: [] },
    meta: wordMeta('ラーメン'),
    isReplySucceeded: true,
  },
): {
  service: WebhookService;
  repo: WebhookRepository;
  lineReplyService: LineReplyService;
} {
  const lineReplyService = {
    processEvent: vi.fn().mockResolvedValue(processEventResult),
  } as unknown as LineReplyService;
  const repo: WebhookRepository = {
    createGourmetLocationLog: vi.fn().mockResolvedValue(undefined),
    createGourmetWordLog: vi.fn().mockResolvedValue(undefined),
    createJobLog: vi.fn().mockResolvedValue(undefined),
  };
  const service = new WebhookService(lineReplyService, repo);
  return { service, repo, lineReplyService };
}

describe('WebhookService.processSync', () => {
  it('イベントを処理し、metaをDB保存してreply/meta/isReplySucceededの配列を返す（JobLogは保存しない）', async () => {
    const { service, repo } = buildService();

    const results = await service.processSync(buildWebhookBody([messageEvent()]), 'genre1');

    expect(results).toEqual([
      { reply: { replyToken: 'token-1', messages: [] }, meta: wordMeta('ラーメン'), isReplySucceeded: true },
    ]);
    expect(vi.mocked(repo.createGourmetWordLog)).toHaveBeenCalledWith(wordMeta('ラーメン'));
    expect(vi.mocked(repo.createJobLog)).not.toHaveBeenCalled();
  });
});
