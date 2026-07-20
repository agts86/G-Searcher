import type { MiddlewareHandler } from "hono";
import { createMiddleware } from "hono/factory";
import { validateSignature } from "@line/bot-sdk";

const SIGNATURE_HEADER = "x-line-signature";

/**
 * 既存.NET側 LineSignatureFilter 相当のHonoミドルウェア。
 * 生ボディを検証に使うため c.req.raw.clone() で複製し、元のリクエストボディは
 * 後続ハンドラー（c.req.json()等）がそのまま読めるようにする。
 * verifySignature=false はローカル開発でLINEからの実リクエストを模擬しない場合のみ使う。
 */
export function createLineSignatureGuard(
	channelSecret: string,
	verifySignature = true,
): MiddlewareHandler {
	return createMiddleware(async (c, next) => {
		if (!verifySignature) {
			await next();
			return;
		}

		const signature = c.req.header(SIGNATURE_HEADER);
		if (!signature) {
			return c.json({ message: "Unauthorized." }, 401);
		}

		const rawBody = await c.req.raw.clone().text();
		if (!validateSignature(rawBody, channelSecret, signature)) {
			return c.json({ message: "Unauthorized." }, 401);
		}

		await next();
	});
}
