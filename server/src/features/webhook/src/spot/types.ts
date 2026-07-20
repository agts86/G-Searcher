import type { Reply } from "../common.types.js";

/** メッセージ種別ごとの検索条件・永続化データを組み立てる際に使う内部表現 */
export type GourmetLogEntry =
	| { type: "location"; lat: number; lng: number }
	| { type: "word"; text: string | null };

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

/** 既存.NET側 LocalEventResultDto と同じ形 */
export interface SpotEventResult {
	reply: Reply;
	meta: PersistedMeta;
	isReplySucceeded: boolean;
}
