export interface NewGourmetLocationLog {
  lat: number;
  lng: number;
}

export interface NewGourmetWordLog {
  text: string | null;
}

export interface NewJobLog {
  id: string;
  isSuccess: boolean;
  contents: string | null;
  info: string | null;
}

/**
 * Webhook機能が必要とする永続化の抽象。
 * 実装（Prisma経由）は @api-ts/infrastructure が提供する。
 */
export interface WebhookRepository {
  createGourmetLocationLog: (log: NewGourmetLocationLog) => Promise<void>;
  createGourmetWordLog: (log: NewGourmetWordLog) => Promise<void>;
  createJobLog: (log: NewJobLog) => Promise<void>;
}
