export interface StoredRefreshToken {
	id: string;
	userName: string;
	tokenHash: string;
	expiresAt: Date;
}

export interface NewRefreshToken {
	userName: string;
	tokenHash: string;
	expiresAt: Date;
}

/**
 * Auth機能が必要とするRefreshToken永続化の抽象。
 * 実装（Prisma経由）は @api/infrastructure が提供する。
 * このパッケージからPrisma/DBクライアントを直接importしない。
 *
 * replaceUserTokens / rotateToken は、既存.NET側の
 * 「RevokeAllRefreshTokensByUserNameAsync + AddRefreshTokenAsync + SaveChangesAsync」
 * （EF Coreの1トランザクションでのアトミックコミット）と同じ整合性を保証する操作として、
 * あえて「削除+作成」をセットにしたメソッドにしている。個別のdelete/createに分けない。
 */
export interface AuthRepository {
	findByTokenHash(tokenHash: string): Promise<StoredRefreshToken | null>;
	/** 指定ユーザーの既存トークンを全削除し、新規トークンを1件作成する（アトミック。ログイン時に使う） */
	replaceUserTokens(userName: string, newToken: NewRefreshToken): Promise<void>;
	/** 指定IDのトークンを削除し、新規トークンを1件作成する（アトミック。リフレッシュのローテーションに使う） */
	rotateToken(oldId: string, newToken: NewRefreshToken): Promise<void>;
	revoke(id: string): Promise<void>;
}
