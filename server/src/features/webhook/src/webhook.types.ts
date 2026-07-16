import type { webhook } from '@line/bot-sdk';

export interface LocalJob {
  id: string;
  webhookBody: webhook.CallbackRequest;
  genreCode: string | undefined;
}

/** メッセージ種別ごとの検索条件・永続化データを組み立てる際に使う内部表現 */
export type GourmetLogEntry =
  | { type: 'location'; lat: number; lng: number }
  | { type: 'word'; text: string | null };

/**
 * 永続化されたGourmetLocationLog/GourmetWordLogそのもの（既存.NET側 Meta 派生クラスと同じ形）。
 * `type`のような判別タグはpersist済みエンティティのJSON表現には含めない
 * （.NET側もRealMoldConverterで実際のプロパティのみをシリアライズするため）。
 */
export type PersistedGourmetLocationLog = {
  id: string;
  lat: number;
  lng: number;
  createdAt: string;
  updatedAt: string;
};

export type PersistedGourmetWordLog = {
  id: string;
  text: string | null;
  createdAt: string;
  updatedAt: string;
};

export type PersistedMeta = PersistedGourmetLocationLog | PersistedGourmetWordLog;

/** 既存.NET側 LineDevSdk.DTO.MessagingAPIs.Reply と同じ形（replyToken + messages） */
export interface LocalReply {
  replyToken: string;
  messages: unknown[];
}

/** 既存.NET側 LocalEventResultDto と同じ形 */
export interface LocalEventResult {
  reply: LocalReply;
  meta: PersistedMeta;
  isReplySucceeded: boolean;
}

export interface LocalJobResult {
  job: LocalJob;
  results: LocalEventResult[];
  isSuccess: boolean;
  errorMessage: string | null;
}

/** バイク駐車場検索版のイベント結果（DB永続化しないためmetaは持たない） */
export interface BikeParkingEventResult {
  reply: LocalReply;
  isReplySucceeded: boolean;
}
