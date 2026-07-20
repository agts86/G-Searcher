import * as jose from "jose";
import { randomUUID } from "node:crypto";

/** .NET側 System.Security.Claims.ClaimTypes.Name の実体文字列。JWTのnameクレームキーとして使う */
export const NAME_CLAIM_TYPE = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

export interface JwtConfig {
	secret: string;
	issuer: string;
	audience: string;
}

export interface CreatedAccessToken {
	token: string;
	expiresAt: Date;
}

const CLOCK_TOLERANCE_SECONDS = 60;

function toSecretKey(secret: string): Uint8Array {
	return new TextEncoder().encode(secret);
}

/** 既存.NET側 AuthService.CreateAccessToken と同じクレーム構成でJWTを発行する */
export async function createAccessToken(
	userName: string,
	config: JwtConfig,
	expiresInSeconds: number,
): Promise<CreatedAccessToken> {
	const expiresAt = new Date(Date.now() + expiresInSeconds * 1000);

	const token = await new jose.SignJWT({ [NAME_CLAIM_TYPE]: userName })
		.setProtectedHeader({ alg: "HS256" })
		.setSubject(userName)
		.setJti(randomUUID())
		.setIssuer(config.issuer)
		.setAudience(config.audience)
		.setExpirationTime(expiresAt)
		.sign(toSecretKey(config.secret));

	return { token, expiresAt };
}

export interface VerifiedAccessToken {
	userName: string;
}

/** 署名・issuer・audience・有効期限（clockSkew 1分許容）を検証し、ユーザー名を返す。不正なら例外を投げる */
export async function verifyAccessToken(
	token: string,
	config: JwtConfig,
): Promise<VerifiedAccessToken> {
	const { payload } = await jose.jwtVerify(token, toSecretKey(config.secret), {
		issuer: config.issuer,
		audience: config.audience,
		clockTolerance: CLOCK_TOLERANCE_SECONDS,
	});

	const userName = payload[NAME_CLAIM_TYPE];
	if (typeof userName !== "string" || userName.length === 0) {
		throw new Error("Unauthorized.");
	}

	return { userName };
}
