import type { PrismaClient } from '@prisma/client';
import type { AuthRepository, NewRefreshToken, StoredRefreshToken } from '@api-ts/features-auth';

/**
 * features/auth の AuthRepository インターフェースをPrisma経由で実装する。
 * replaceUserTokens / rotateToken は $transaction で「削除+作成」をアトミックに実行し、
 * 既存.NET側の EF Core SaveChangesAsync（1トランザクションでのコミット）と同じ整合性を保証する。
 */
export class PrismaAuthRepository implements AuthRepository {
  constructor(private readonly prisma: PrismaClient) {}

  async findByTokenHash(tokenHash: string): Promise<StoredRefreshToken | null> {
    const row = await this.prisma.refreshToken.findUnique({ where: { tokenHash } });
    if (!row) return null;
    return { id: row.id, userName: row.userName, tokenHash: row.tokenHash, expiresAt: row.expiresAt };
  }

  async replaceUserTokens(userName: string, newToken: NewRefreshToken): Promise<void> {
    await this.prisma.$transaction([
      this.prisma.refreshToken.deleteMany({ where: { userName } }),
      this.prisma.refreshToken.create({ data: newToken }),
    ]);
  }

  async rotateToken(oldId: string, newToken: NewRefreshToken): Promise<void> {
    await this.prisma.$transaction([
      this.prisma.refreshToken.delete({ where: { id: oldId } }),
      this.prisma.refreshToken.create({ data: newToken }),
    ]);
  }

  async revoke(id: string): Promise<void> {
    await this.prisma.refreshToken.delete({ where: { id } }).catch(() => undefined);
  }
}
