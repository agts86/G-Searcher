import { LineBotClient, type messagingApi } from '@line/bot-sdk';
import type { LineReplyClient } from '@api/features-webhook';

/** LINE Messaging APIへの返信実装。@line/bot-sdk の LineBotClient を使う */
export class LineReplyClientImpl implements LineReplyClient {
  private readonly client: LineBotClient;

  constructor(channelAccessToken: string) {
    this.client = LineBotClient.fromChannelAccessToken({ channelAccessToken });
  }

  async send(replyToken: string, messages: unknown[]): Promise<boolean> {
    try {
      await this.client.replyMessage({ replyToken, messages: messages as messagingApi.Message[] });
      return true;
    } catch (err) {
      // eslint-disable-next-line no-console -- LINE返信失敗は現状DB/JobLogに残らないため、調査用に最低限ログへ出す
      console.error(`LINE reply failed: replyToken=${replyToken}`, err);
      return false;
    }
  }
}
