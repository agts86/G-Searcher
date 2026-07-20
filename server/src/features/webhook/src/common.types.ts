/** 既存.NET側 LineDevSdk.DTO.MessagingAPIs.Reply と同じ形（replyToken + messages） */
export interface Reply {
	replyToken: string;
	messages: unknown[];
}
