export interface GourmetLocationLogRow {
	id: string;
	lat: number;
	lng: number;
	createdAt: Date;
	updatedAt: Date;
}

export interface GourmetWordLogRow {
	id: string;
	text: string | null;
	createdAt: Date;
	updatedAt: Date;
}

export interface ErrorLogRow {
	id: string;
	contents: string;
	createdAt: Date;
	updatedAt: Date;
}

export interface JobLogRow {
	id: string;
	isSuccess: boolean;
	contents: string | null;
	info: string | null;
	createdAt: Date;
	updatedAt: Date;
}

/**
 * Managed機能が必要とする読み取り専用の永続化の抽象。
 * 実装（Prisma経由）は @api/infrastructure が提供する。
 * 全メソッドとも既存.NET側 ManagedRepository と同じく createdAt 降順の全件取得。
 */
export interface ManagedRepository {
	findGourmetLocationLogs(): Promise<GourmetLocationLogRow[]>;
	findGourmetWordLogs(): Promise<GourmetWordLogRow[]>;
	findErrorLogs(): Promise<ErrorLogRow[]>;
	findJobLogs(): Promise<JobLogRow[]>;
}
