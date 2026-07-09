import { LineBotClient, type messagingApi } from '@line/bot-sdk';
import type { LineReplyClient, CarouselColumn } from '@api-ts/features-webhook';

const NOT_FOUND_ALT_TEXT = '検索結果';
const DETAIL_LABEL = '詳細を見る';

/** LINE Messaging APIへの返信実装。@line/bot-sdk の LineBotClient を使う */
export class LineReplyClientImpl implements LineReplyClient {
  private readonly client: LineBotClient;

  constructor(channelAccessToken: string) {
    this.client = LineBotClient.fromChannelAccessToken({ channelAccessToken });
  }

  async replyCarousel(replyToken: string, columns: CarouselColumn[]): Promise<boolean> {
    const message: messagingApi.TemplateMessage = {
      type: 'template',
      altText: NOT_FOUND_ALT_TEXT,
      template: {
        type: 'carousel',
        columns: columns.map((column) => ({
          title: column.title,
          text: column.text,
          actions: [{ type: 'uri', label: DETAIL_LABEL, uri: column.detailUrl }],
        })),
      },
    };
    return this.send(replyToken, [message]);
  }

  async replyText(replyToken: string, text: string): Promise<boolean> {
    const message: messagingApi.TextMessage = { type: 'text', text };
    return this.send(replyToken, [message]);
  }

  private async send(replyToken: string, messages: messagingApi.Message[]): Promise<boolean> {
    try {
      await this.client.replyMessage({ replyToken, messages });
      return true;
    } catch (err) {
      // eslint-disable-next-line no-console -- LINE返信失敗は現状DB/JobLogに残らないため、調査用に最低限ログへ出す
      console.error(`LINE reply failed: replyToken=${replyToken}`, err);
      return false;
    }
  }
}
