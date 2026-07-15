import { describe, expect, it, vi } from 'vitest';
import type { webhook } from '@line/bot-sdk';
import { BikeParkingReplyService, NOT_FOUND_TEXT } from '../src/bike-parking-reply.service.js';
import type { BikeParkingClient, BikeParkingSpot } from '../src/bike-parking-client.js';
import type { LineReplyClient } from '../src/line-reply-client.js';

function callbackRequest(events: webhook.Event[]): webhook.CallbackRequest {
  return { destination: 'U1', events };
}

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

function spot(detailUrl: string, name: string): BikeParkingSpot {
  return {
    name,
    address: `${name}の住所`,
    fee: `${name}の料金`,
    holiday: 'なし',
    detailUrl,
    lat: 35.5,
    lng: 139.5,
  };
}

function buildService(options: {
  searchResults?: BikeParkingSpot[];
  skipLineApiCall?: boolean;
}): { service: BikeParkingReplyService; bikeParkingClient: BikeParkingClient; lineReplyClient: LineReplyClient } {
  const bikeParkingClient: BikeParkingClient = {
    search: vi.fn().mockResolvedValue(options.searchResults ?? []),
  };
  const lineReplyClient: LineReplyClient = {
    send: vi.fn().mockResolvedValue(true),
  };
  const service = new BikeParkingReplyService(bikeParkingClient, lineReplyClient, options.skipLineApiCall ?? false);
  return { service, bikeParkingClient, lineReplyClient };
}

describe('BikeParkingReplyService.processEvent', () => {
  it('テキストメッセージは全角スペースを半角に置換して文字列検索する', async () => {
    const { service, bikeParkingClient } = buildService({ searchResults: [spot('/society/parking/area13/p-1.html', '駐車場A')] });

    await service.processEvent(textMessageEvent('スカイツリー　周辺'));

    expect(vi.mocked(bikeParkingClient.search)).toHaveBeenCalledWith({ query: 'スカイツリー 周辺' });
  });

  it('ロケーションメッセージは緯度経度で位置情報検索する', async () => {
    const { service, bikeParkingClient } = buildService({ searchResults: [spot('/society/parking/area13/p-1.html', '駐車場A')] });

    await service.processEvent(locationMessageEvent(35.5, 139.5));

    expect(vi.mocked(bikeParkingClient.search)).toHaveBeenCalledWith({ lat: 35.5, lng: 139.5 });
  });

  it('reply.replyTokenは元イベントのreplyTokenと一致する', async () => {
    const { service } = buildService({ searchResults: [spot('/society/parking/area13/p-1.html', '駐車場A')] });

    const result = await service.processEvent(textMessageEvent('スカイツリー'));

    expect(result?.reply.replyToken).toBe('reply-token-1');
  });

  it('検索結果0件ならNotFoundテキストで返信する', async () => {
    const { service, lineReplyClient } = buildService({ searchResults: [] });

    const result = await service.processEvent(textMessageEvent('該当なし'));

    const expectedMessages = [{ type: 'text', text: NOT_FOUND_TEXT }];
    expect(vi.mocked(lineReplyClient.send)).toHaveBeenCalledWith('reply-token-1', expectedMessages);
    expect(result?.reply.messages).toEqual(expectedMessages);
  });

  it('検索結果があればカルーセルで返信し、詳細URLは絶対URLへ変換する', async () => {
    const { service, lineReplyClient } = buildService({ searchResults: [spot('/society/parking/area13/p-1.html', '駐車場A')] });

    const result = await service.processEvent(textMessageEvent('スカイツリー'));

    const expectedMessages = [
      {
        type: 'template',
        altText: '検索結果',
        template: {
          type: 'carousel',
          columns: [
            {
              title: '駐車場A',
              text: '駐車場Aの住所\n駐車場Aの料金',
              actions: [
                { type: 'uri', label: '詳細を見る', uri: 'https://www.jmpsa.or.jp/society/parking/area13/p-1.html' },
              ],
            },
          ],
        },
      },
    ];
    expect(vi.mocked(lineReplyClient.send)).toHaveBeenCalledWith('reply-token-1', expectedMessages);
    expect(result?.reply.messages).toEqual(expectedMessages);
  });

  it('detailUrlが無い結果はカルーセルから除外し、全て除外されたらNotFoundテキストを返信する', async () => {
    const noUrlSpot: BikeParkingSpot = { name: '駐車場A', address: '住所', fee: null, holiday: null, detailUrl: '', lat: null, lng: null };
    const { service, lineReplyClient } = buildService({ searchResults: [noUrlSpot] });

    await service.processEvent(textMessageEvent('スカイツリー'));

    expect(vi.mocked(lineReplyClient.send)).toHaveBeenCalledWith('reply-token-1', [{ type: 'text', text: NOT_FOUND_TEXT }]);
  });

  it('同じdetailUrlの結果は重複排除し、最大10件までにする', async () => {
    const results = [
      spot('/society/parking/area13/p-1.html', '駐車場A-1'),
      spot('/society/parking/area13/p-1.html', '駐車場A-2'),
      ...Array.from({ length: 12 }, (_, i) => spot(`/society/parking/area13/p-${i + 2}.html`, `駐車場${i + 2}`)),
    ];
    const { service, lineReplyClient } = buildService({ searchResults: results });

    await service.processEvent(textMessageEvent('スカイツリー'));

    const [, messages] = vi.mocked(lineReplyClient.send).mock.calls[0];
    const [templateMessage] = messages as [{ template: { columns: { title: string }[] } }];
    expect(templateMessage.template.columns).toHaveLength(10);
    expect(templateMessage.template.columns[0].title).toBe('駐車場A-1');
  });

  it('isReplySucceededは返信結果を反映する', async () => {
    const { service, lineReplyClient } = buildService({ searchResults: [spot('/society/parking/area13/p-1.html', '駐車場A')] });
    vi.mocked(lineReplyClient.send).mockResolvedValue(false);

    const result = await service.processEvent(textMessageEvent('スカイツリー'));

    expect(result?.isReplySucceeded).toBe(false);
  });

  it('skipLineApiCall=trueなら実際の返信APIを呼ばずisReplySucceeded=trueを返す（開発環境向け）', async () => {
    const { service, lineReplyClient } = buildService({
      searchResults: [spot('/society/parking/area13/p-1.html', '駐車場A')],
      skipLineApiCall: true,
    });

    const result = await service.processEvent(textMessageEvent('スカイツリー'));

    expect(vi.mocked(lineReplyClient.send)).not.toHaveBeenCalled();
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

    const result = await service.processEvent(followEvent);

    expect(result).toBeNull();
  });
});

describe('BikeParkingReplyService.processEvents', () => {
  it('全イベントを処理し、null（非対象イベント）は結果から除外する', async () => {
    const { service } = buildService({ searchResults: [spot('/society/parking/area13/p-1.html', '駐車場A')] });
    const followEvent = {
      type: 'follow',
      timestamp: 0,
      mode: 'active',
      webhookEventId: 'we-3',
      deliveryContext: { isRedelivery: false },
    } as unknown as webhook.Event;

    const results = await service.processEvents(callbackRequest([textMessageEvent('スカイツリー'), followEvent]));

    expect(results).toHaveLength(1);
    expect(results[0].reply.replyToken).toBe('reply-token-1');
  });

  it('イベントが無ければ空配列を返す', async () => {
    const { service } = buildService({});

    const results = await service.processEvents(callbackRequest([]));

    expect(results).toEqual([]);
  });
});
