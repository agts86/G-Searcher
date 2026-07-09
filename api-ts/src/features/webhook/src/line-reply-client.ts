export interface CarouselColumn {
  title: string;
  text: string;
  detailUrl: string;
}

/**
 * LINE Messaging APIへの返信の抽象。実装は @line/bot-sdk の LineBotClient を使い
 * @api-ts/infrastructure が提供する。
 */
export interface LineReplyClient {
  replyCarousel: (replyToken: string, columns: CarouselColumn[]) => Promise<boolean>;
  replyText: (replyToken: string, text: string) => Promise<boolean>;
}
