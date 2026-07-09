import { describe, expect, it, vi } from 'vitest';
import type { webhook } from '@line/bot-sdk';
import { LineReplyService, NOT_FOUND_TEXT } from '../src/line-reply.service.js';
import type { YolpClient, YolpFeature } from '../src/yolp-client.js';
import type { LineReplyClient } from '../src/line-reply-client.js';

function textMessageEvent(text: string): webhook.MessageEvent {
  return {
    type: 'message',
    replyToken: 'reply-token-1',
    message: { type: 'text', id: 'msg-1', text, quoteToken: 'q1' },
    timestamp: 0,
    mode: 'active',
    webhookEventId: 'we-1',
    deliveryContext: { isRedelivery: false },
  };
}

function locationMessageEvent(latitude: number, longitude: number): webhook.MessageEvent {
  return {
    type: 'message',
    replyToken: 'reply-token-2',
    message: { type: 'location', id: 'msg-2', latitude, longitude },
    timestamp: 0,
    mode: 'active',
    webhookEventId: 'we-2',
    deliveryContext: { isRedelivery: false },
  };
}

function feature(gid: string, name: string): YolpFeature {
  return { gid, name, address: `${name}の住所`, detailUrl: `https://example.com/${gid}` };
}

function buildService(options: {
  searchResults?: YolpFeature[];
  skipLineApiCall?: boolean;
}): { service: LineReplyService; yolpClient: YolpClient; lineReplyClient: LineReplyClient } {
  const yolpClient: YolpClient = {
    searchLocal: vi.fn().mockResolvedValue(options.searchResults ?? []),
  };
  const lineReplyClient: LineReplyClient = {
    replyCarousel: vi.fn().mockResolvedValue(true),
    replyText: vi.fn().mockResolvedValue(true),
  };
  const service = new LineReplyService(yolpClient, lineReplyClient, options.skipLineApiCall ?? false);
  return { service, yolpClient, lineReplyClient };
}

describe('LineReplyService.processEvent', () => {
  it('テキストメッセージはGourmetWordLogに変換し、全角スペースを半角に置換してYOLP検索する', async () => {
    const { service, yolpClient } = buildService({ searchResults: [feature('g1', '店A')] });

    const result = await service.processEvent(textMessageEvent('ラーメン　うどん'), 'genre1');

    expect(vi.mocked(yolpClient.searchLocal)).toHaveBeenCalledWith({
      genreCode: 'genre1',
      location: { query: 'ラーメン うどん' },
    });
    expect(result?.meta).toEqual({ type: 'word', text: 'ラーメン　うどん' });
  });

  it('ロケーションメッセージはGourmetLocationLogに変換し、緯度経度でYOLP検索する', async () => {
    const { service, yolpClient } = buildService({ searchResults: [feature('g1', '店A')] });

    const result = await service.processEvent(locationMessageEvent(35.5, 139.5), 'genre1');

    expect(vi.mocked(yolpClient.searchLocal)).toHaveBeenCalledWith({
      genreCode: 'genre1',
      location: { lat: 35.5, lon: 139.5 },
    });
    expect(result?.meta).toEqual({ type: 'location', lat: 35.5, lng: 139.5 });
  });

  it('検索結果0件ならNotFoundテキストを返信する', async () => {
    const { service, lineReplyClient } = buildService({ searchResults: [] });

    await service.processEvent(textMessageEvent('存在しない店'), 'genre1');

    expect(vi.mocked(lineReplyClient.replyText)).toHaveBeenCalledWith('reply-token-1', NOT_FOUND_TEXT);
  });

  it('検索結果があればカルーセルで返信する', async () => {
    const { service, lineReplyClient } = buildService({ searchResults: [feature('g1', '店A')] });

    await service.processEvent(textMessageEvent('ラーメン'), 'genre1');

    expect(vi.mocked(lineReplyClient.replyCarousel)).toHaveBeenCalledWith('reply-token-1', [
      { title: '店A', text: '店Aの住所', detailUrl: 'https://example.com/g1' },
    ]);
  });

  it('detailUrlが無い結果はカルーセルから除外し、全て除外されたらNotFoundテキストを返信する', async () => {
    const noUrlFeature: YolpFeature = { gid: 'g1', name: '店A', address: '店Aの住所', detailUrl: null };
    const { service, lineReplyClient } = buildService({ searchResults: [noUrlFeature] });

    await service.processEvent(textMessageEvent('ラーメン'), 'genre1');

    expect(vi.mocked(lineReplyClient.replyCarousel)).not.toHaveBeenCalled();
    expect(vi.mocked(lineReplyClient.replyText)).toHaveBeenCalledWith('reply-token-1', NOT_FOUND_TEXT);
  });

  it('detailUrlが無い結果は除いて、有効な結果だけでカルーセルを返信する', async () => {
    const noUrlFeature: YolpFeature = { gid: 'g1', name: '店A', address: '店Aの住所', detailUrl: null };
    const { service, lineReplyClient } = buildService({ searchResults: [noUrlFeature, feature('g2', '店B')] });

    await service.processEvent(textMessageEvent('ラーメン'), 'genre1');

    expect(vi.mocked(lineReplyClient.replyCarousel)).toHaveBeenCalledWith('reply-token-1', [
      { title: '店B', text: '店Bの住所', detailUrl: 'https://example.com/g2' },
    ]);
  });

  it('同じgidの結果は重複排除し、最大10件までにする', async () => {
    const results = [
      feature('g1', '店A-1'),
      feature('g1', '店A-2'),
      ...Array.from({ length: 12 }, (_, i) => feature(`g${i + 2}`, `店${i + 2}`)),
    ];
    const { service, lineReplyClient } = buildService({ searchResults: results });

    await service.processEvent(textMessageEvent('ラーメン'), 'genre1');

    const columns = vi.mocked(lineReplyClient.replyCarousel).mock.calls[0][1];
    expect(columns).toHaveLength(10);
    expect(columns[0].title).toBe('店A-1');
  });

  it('isReplySucceededは返信結果を反映する', async () => {
    const { service, lineReplyClient } = buildService({ searchResults: [feature('g1', '店A')] });
    vi.mocked(lineReplyClient.replyCarousel).mockResolvedValue(false);

    const result = await service.processEvent(textMessageEvent('ラーメン'), 'genre1');

    expect(result?.isReplySucceeded).toBe(false);
  });

  it('skipLineApiCall=trueなら実際の返信APIを呼ばずisReplySucceeded=trueを返す（開発環境向け）', async () => {
    const { service, lineReplyClient } = buildService({ searchResults: [feature('g1', '店A')], skipLineApiCall: true });

    const result = await service.processEvent(textMessageEvent('ラーメン'), 'genre1');

    expect(vi.mocked(lineReplyClient.replyCarousel)).not.toHaveBeenCalled();
    expect(vi.mocked(lineReplyClient.replyText)).not.toHaveBeenCalled();
    expect(result?.isReplySucceeded).toBe(true);
  });

  it('メッセージイベント以外はnullを返す（スキップ）', async () => {
    const { service } = buildService({});
    const followEvent = {
      type: 'follow',
      timestamp: 0,
      mode: 'active',
      webhookEventId: 'we-3',
      deliveryContext: { isRedelivery: false },
    } as unknown as webhook.Event;

    const result = await service.processEvent(followEvent, 'genre1');

    expect(result).toBeNull();
  });
});
