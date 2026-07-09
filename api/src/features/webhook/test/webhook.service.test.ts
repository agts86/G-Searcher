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

describe('WebhookService.enqueueLocalJob', () => {
  it('IDを採番したジョブを返す（DB書込・YOLP呼び出しは一切しない）', () => {
    const { service, repo, lineReplyService } = buildService();

    const job = service.enqueueLocalJob(buildWebhookBody([messageEvent()]), 'genre1');

    expect(job.id).toBeTruthy();
    expect(job.genreCode).toBe('genre1');
    expect(vi.mocked(repo.createJobLog)).not.toHaveBeenCalled();
    expect(vi.mocked(lineReplyService.processEvent)).not.toHaveBeenCalled();
  });
});

describe('WebhookService.processJob', () => {
  it('各イベントをLineReplyServiceで処理し、成功時はisSuccess=trueのJobResultを返す', async () => {
    const { service } = buildService();
    const job = { id: 'job-1', webhookBody: buildWebhookBody([messageEvent()]), genreCode: 'genre1' };

    const result = await service.processJob(job);

    expect(result.isSuccess).toBe(true);
    expect(result.errorMessage).toBeNull();
    expect(result.results).toHaveLength(1);
  });

  it('処理中に例外が起きたらisSuccess=falseでerrorMessageを設定する', async () => {
    const lineReplyService = { processEvent: vi.fn().mockRejectedValue(new Error('yolp down')) } as unknown as LineReplyService;
    const repo: WebhookRepository = {
      createGourmetLocationLog: vi.fn(),
      createGourmetWordLog: vi.fn(),
      createJobLog: vi.fn(),
    };
    const service = new WebhookService(lineReplyService, repo);
    const job = { id: 'job-1', webhookBody: buildWebhookBody([messageEvent()]), genreCode: 'genre1' };

    const result = await service.processJob(job);

    expect(result.isSuccess).toBe(false);
    expect(result.errorMessage).toBe('yolp down');
  });
});

describe('WebhookService.persistJobResult', () => {
  it('resultsのmetaをGourmet*Logへ、JobをJobLogへ保存する', async () => {
    const { service, repo } = buildService();
    const job = { id: 'job-1', webhookBody: buildWebhookBody([messageEvent()]), genreCode: 'genre1' };
    const meta = wordMeta('ラーメン');
    const jobResult = {
      job,
      results: [{ reply: { replyToken: 'token-1', messages: [] }, meta, isReplySucceeded: true }],
      isSuccess: true,
      errorMessage: null,
    };

    await service.persistJobResult(jobResult);

    expect(vi.mocked(repo.createGourmetWordLog)).toHaveBeenCalledWith(meta);
    expect(vi.mocked(repo.createJobLog)).toHaveBeenCalledWith({
      id: 'job-1',
      isSuccess: true,
      contents: JSON.stringify({ id: 'job-1', webHook: job.webhookBody, genreCode: 'genre1' }),
      info: null,
    });
  });
});

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
