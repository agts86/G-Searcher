import type { PrismaClient } from '@prisma/client';
import type { WebhookRepository, NewGourmetLocationLog, NewGourmetWordLog, NewJobLog } from '@api-ts/features-webhook';

/** features/webhook の WebhookRepository インターフェースをPrisma経由で実装する */
export class PrismaWebhookRepository implements WebhookRepository {
  constructor(private readonly prisma: PrismaClient) {}

  async createGourmetLocationLog(log: NewGourmetLocationLog): Promise<void> {
    await this.prisma.gourmetLocationLog.create({ data: log });
  }

  async createGourmetWordLog(log: NewGourmetWordLog): Promise<void> {
    await this.prisma.gourmetWordLog.create({ data: log });
  }

  async createJobLog(log: NewJobLog): Promise<void> {
    await this.prisma.jobLog.create({ data: log });
  }
}
