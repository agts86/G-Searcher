import type { PrismaClient } from '@prisma/client';
import type { WebhookRepository, NewGourmetLocationLog, NewGourmetWordLog, NewJobLog } from '@api/features-webhook';

/**
 * features/webhook の WebhookRepository インターフェースをPrisma経由で実装する。
 * id/createdAt/updatedAtはPrisma側の@default/@updatedAtに任せず、呼び出し側
 * （LineReplyService）で既に確定した値をそのまま書き込む
 * （レスポンスに含めたid/timestampとDBの値を完全に一致させるため）。
 */
export class PrismaWebhookRepository implements WebhookRepository {
  constructor(private readonly prisma: PrismaClient) {}

  async createGourmetLocationLog(log: NewGourmetLocationLog): Promise<void> {
    await this.prisma.gourmetLocationLog.create({
      data: { id: log.id, lat: log.lat, lng: log.lng, createdAt: new Date(log.createdAt), updatedAt: new Date(log.updatedAt) },
    });
  }

  async createGourmetWordLog(log: NewGourmetWordLog): Promise<void> {
    await this.prisma.gourmetWordLog.create({
      data: { id: log.id, text: log.text, createdAt: new Date(log.createdAt), updatedAt: new Date(log.updatedAt) },
    });
  }

  async createJobLog(log: NewJobLog): Promise<void> {
    await this.prisma.jobLog.create({ data: log });
  }
}
