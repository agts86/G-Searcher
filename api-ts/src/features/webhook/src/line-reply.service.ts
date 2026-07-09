import type { webhook } from '@line/bot-sdk';
import type { YolpClient, YolpFeature, YolpLocation } from './yolp-client.js';
import type { LineReplyClient, CarouselColumn } from './line-reply-client.js';
import type { GourmetLogEntry, LocalEventResult } from './webhook.types.js';

export const NOT_FOUND_TEXT = 'ごめんなさい。。見つかりませんでした。。';
const MAX_CAROUSEL_COLUMNS = 10;
// 全角スペース(U+3000)を検出する。リテラル文字だとESLintのno-irregular-whitespaceに
// 引っかかるためコードポイント(0x3000)から動的に生成する。
const FULL_WIDTH_SPACE = new RegExp(String.fromCharCode(0x3000), 'g');

function isMessageEvent(event: webhook.Event): event is webhook.MessageEvent {
  return event.type === 'message';
}

function toGourmetLogEntry(message: webhook.MessageContent): GourmetLogEntry | null {
  if (message.type === 'text') {
    return { type: 'word', text: message.text };
  }
  if (message.type === 'location') {
    return { type: 'location', lat: message.latitude, lng: message.longitude };
  }
  return null;
}

function toQueryLocation(meta: GourmetLogEntry): YolpLocation {
  if (meta.type === 'location') {
    return { lat: meta.lat, lon: meta.lng };
  }
  return { query: (meta.text ?? '').replace(FULL_WIDTH_SPACE, ' ') };
}

function dedupeAndCap(features: YolpFeature[]): YolpFeature[] {
  const seen = new Set<string>();
  const result: YolpFeature[] = [];
  for (const feature of features) {
    if (seen.has(feature.gid)) continue;
    seen.add(feature.gid);
    result.push(feature);
    if (result.length >= MAX_CAROUSEL_COLUMNS) break;
  }
  return result;
}

function toCarouselColumns(features: YolpFeature[]): CarouselColumn[] {
  return dedupeAndCap(features).map((f) => ({
    title: f.name,
    text: f.address ?? '',
    detailUrl: f.detailUrl ?? '',
  }));
}

/** 既存.NET側 LineReplyService と同じ振る舞い（メッセージ解析→YOLP検索→カルーセル/NotFound返信） */
export class LineReplyService {
  constructor(
    private readonly yolpClient: YolpClient,
    private readonly lineReplyClient: LineReplyClient,
    private readonly skipLineApiCall: boolean,
  ) {}

  processEvent = async (event: webhook.Event, genreCode: string | undefined): Promise<LocalEventResult | null> => {
    if (!isMessageEvent(event) || !event.replyToken) {
      return null;
    }

    const meta = toGourmetLogEntry(event.message);
    if (!meta) {
      return null;
    }

    const features = await this.yolpClient.searchLocal({
      genreCode: genreCode ?? '',
      location: toQueryLocation(meta),
    });

    const isReplySucceeded = await this.reply(event.replyToken, features);

    return { meta, isReplySucceeded };
  };

  private async reply(replyToken: string, features: YolpFeature[]): Promise<boolean> {
    // 既存.NET側 Env.IsDevelopment() 時にLINE API呼び出し自体をスキップする挙動と同じ
    if (this.skipLineApiCall) {
      return true;
    }

    if (features.length === 0) {
      return this.lineReplyClient.replyText(replyToken, NOT_FOUND_TEXT);
    }

    return this.lineReplyClient.replyCarousel(replyToken, toCarouselColumns(features));
  }
}
