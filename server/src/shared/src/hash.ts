import { createHash } from "node:crypto";

/**
 * .NET側 Convert.ToHexString(SHA256.HashData(...)) と同じ形式（大文字16進数）でハッシュ化する。
 * カットオーバー時点で発行済みのリフレッシュトークンとの互換性のため、大文字で統一する。
 */
export function hashToken(token: string): string {
	return createHash("sha256").update(token, "utf8").digest("hex").toUpperCase();
}
