export interface CarouselColumn {
  title: string;
  text: string;
  detailUrl: string;
}

/**
 * LINE Messaging APIへの返信の抽象。実装は @line/bot-sdk の LineBotClient を使い
 * @api-ts/infrastructure が提供する。
 * メッセージ本文の組み立て（カルーセル/テキスト）はfeature層（LineReplyService）が担い、
 * ここは組み立て済みmessagesをそのまま送るだけの薄いsenderにする
 * （レスポンスに実際に送った内容をそのまま含める必要があるため、組み立てをfeature層に寄せている）。
 */
export interface LineReplyClient {
  send: (replyToken: string, messages: unknown[]) => Promise<boolean>;
}
