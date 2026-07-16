export interface NewGourmetLocationLog {
  id: string;
  lat: number;
  lng: number;
  createdAt: string;
  updatedAt: string;
}

export interface NewGourmetWordLog {
  id: string;
  text: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface NewJobLog {
  id: string;
  isSuccess: boolean;
  contents: string | null;
  info: string | null;
}

/**
 * Webhook機能が必要とする永続化の抽象。
 * 実装（Prisma経由）は @api/infrastructure が提供する。
 * id/createdAt/updatedAtは呼び出し側（LineReplyService）で既に確定済みの値を渡す
 * （既存.NET側 ConvertGourmetLog でIdを先に発番し、SaveChangesAsync時にタイムスタンプを
 * 付与してからレスポンスへ含める、という流れと同じ値をレスポンスとDBの両方で共有するため）。
 */
export interface WebhookRepository {
  createGourmetLocationLog: (log: NewGourmetLocationLog) => Promise<void>;
  createGourmetWordLog: (log: NewGourmetWordLog) => Promise<void>;
  createJobLog: (log: NewJobLog) => Promise<void>;
}
