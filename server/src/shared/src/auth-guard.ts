import type { MiddlewareHandler } from "hono";
import { createMiddleware } from "hono/factory";
import { getCookie } from "hono/cookie";
import { AUTH_COOKIE_NAME } from "./cookies.js";
import { verifyAccessToken, type JwtConfig } from "./jwt.js";

export type AuthGuardVariables = { userName: string };

/**
 * 既存.NET側 [Authorize] 相当のHonoミドルウェア。
 * linewebhook_auth Cookieのアクセストークンを検証し、成功時は c.set('userName', ...) してnext()を呼ぶ。
 * 失敗時は401を返しnext()を呼ばない。
 */
export function createAuthGuard(
	jwtConfig: JwtConfig,
): MiddlewareHandler<{ Variables: AuthGuardVariables }> {
	return createMiddleware<{ Variables: AuthGuardVariables }>(async (c, next) => {
		const accessToken = getCookie(c, AUTH_COOKIE_NAME);
		if (!accessToken) {
			return c.json({ message: "Unauthorized." }, 401);
		}

		try {
			const { userName } = await verifyAccessToken(accessToken, jwtConfig);
			c.set("userName", userName);
		} catch {
			return c.json({ message: "Unauthorized." }, 401);
		}

		await next();
	});
}
