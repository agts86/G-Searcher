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
    send: vi.fn().mockResolvedValue(true),
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
    expect(result?.meta).toMatchObject({ text: 'ラーメン　うどん' });
    expect(typeof result?.meta.id).toBe('string');
    expect(result?.meta.createdAt).toBe(result?.meta.updatedAt);
  });

  it('ロケーションメッセージはGourmetLocationLogに変換し、緯度経度でYOLP検索する', async () => {
    const { service, yolpClient } = buildService({ searchResults: [feature('g1', '店A')] });

    const result = await service.processEvent(locationMessageEvent(35.5, 139.5), 'genre1');

    expect(vi.mocked(yolpClient.searchLocal)).toHaveBeenCalledWith({
      genreCode: 'genre1',
      location: { lat: 35.5, lon: 139.5 },
    });
    expect(result?.meta).toMatchObject({ lat: 35.5, lng: 139.5 });
    expect(typeof result?.meta.id).toBe('string');
  });

  it('reply.replyTokenは元イベントのreplyTokenと一致する', async () => {
    const { service } = buildService({ searchResults: [feature('g1', '店A')] });

    const result = await service.processEvent(textMessageEvent('ラーメン'), 'genre1');

    expect(result?.reply.replyToken).toBe('reply-token-1');
  });

  it('検索結果0件ならNotFoundテキストで返信し、reply.messagesにも同じ内容を含める', async () => {
    const { service, lineReplyClient } = buildService({ searchResults: [] });

    const result = await service.processEvent(textMessageEvent('存在しない店'), 'genre1');

    const expectedMessages = [{ type: 'text', text: NOT_FOUND_TEXT }];
    expect(vi.mocked(lineReplyClient.send)).toHaveBeenCalledWith('reply-token-1', expectedMessages);
    expect(result?.reply.messages).toEqual(expectedMessages);
  });

  it('検索結果があればカルーセルで返信し、reply.messagesにも同じ内容を含める', async () => {
    const { service, lineReplyClient } = buildService({ searchResults: [feature('g1', '店A')] });

    const result = await service.processEvent(textMessageEvent('ラーメン'), 'genre1');

    const expectedMessages = [
      {
        type: 'template',
        altText: '検索結果',
        template: {
          type: 'carousel',
          columns: [
            {
              title: '店A',
              text: '店Aの住所',
              actions: [{ type: 'uri', label: '詳細を見る', uri: 'https://example.com/g1' }],
            },
          ],
        },
      },
    ];
    expect(vi.mocked(lineReplyClient.send)).toHaveBeenCalledWith('reply-token-1', expectedMessages);
    expect(result?.reply.messages).toEqual(expectedMessages);
  });

  it('detailUrlが無い結果はカルーセルから除外し、全て除外されたらNotFoundテキストを返信する', async () => {
    const noUrlFeature: YolpFeature = { gid: 'g1', name: '店A', address: '店Aの住所', detailUrl: null };
    const { service, lineReplyClient } = buildService({ searchResults: [noUrlFeature] });

    await service.processEvent(textMessageEvent('ラーメン'), 'genre1');

    expect(vi.mocked(lineReplyClient.send)).toHaveBeenCalledWith('reply-token-1', [{ type: 'text', text: NOT_FOUND_TEXT }]);
  });

  it('detailUrlが無い結果は除いて、有効な結果だけでカルーセルを返信する', async () => {
    const noUrlFeature: YolpFeature = { gid: 'g1', name: '店A', address: '店Aの住所', detailUrl: null };
    const { service, lineReplyClient } = buildService({ searchResults: [noUrlFeature, feature('g2', '店B')] });

    await service.processEvent(textMessageEvent('ラーメン'), 'genre1');

    const [, messages] = vi.mocked(lineReplyClient.send).mock.calls[0];
    const [templateMessage] = messages as [{ template: { columns: unknown[] } }];
    expect(templateMessage.template.columns).toHaveLength(1);
  });

  it('同じgidの結果は重複排除し、最大10件までにする', async () => {
    const results = [
      feature('g1', '店A-1'),
      feature('g1', '店A-2'),
      ...Array.from({ length: 12 }, (_, i) => feature(`g${i + 2}`, `店${i + 2}`)),
    ];
    const { service, lineReplyClient } = buildService({ searchResults: results });

    await service.processEvent(textMessageEvent('ラーメン'), 'genre1');

    const [, messages] = vi.mocked(lineReplyClient.send).mock.calls[0];
    const [templateMessage] = messages as [{ template: { columns: { title: string }[] } }];
    expect(templateMessage.template.columns).toHaveLength(10);
    expect(templateMessage.template.columns[0].title).toBe('店A-1');
  });

  it('isReplySucceededは返信結果を反映する', async () => {
    const { service, lineReplyClient } = buildService({ searchResults: [feature('g1', '店A')] });
    vi.mocked(lineReplyClient.send).mockResolvedValue(false);

    const result = await service.processEvent(textMessageEvent('ラーメン'), 'genre1');

    expect(result?.isReplySucceeded).toBe(false);
  });

  it('skipLineApiCall=trueなら実際の返信APIを呼ばずisReplySucceeded=trueを返す（開発環境向け）', async () => {
    const { service, lineReplyClient } = buildService({ searchResults: [feature('g1', '店A')], skipLineApiCall: true });

    const result = await service.processEvent(textMessageEvent('ラーメン'), 'genre1');

    expect(vi.mocked(lineReplyClient.send)).not.toHaveBeenCalled();
    expect(result?.isReplySucceeded).toBe(true);
    expect(result?.reply.messages).toHaveLength(1);
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
