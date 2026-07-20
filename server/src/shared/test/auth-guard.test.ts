import { describe, expect, it } from "vitest";
import { Hono } from "hono";
import { createAuthGuard } from "../src/auth-guard.js";
import { createAccessToken } from "../src/jwt.js";
import { AUTH_COOKIE_NAME } from "../src/cookies.js";

const jwtConfig = {
	secret: "test-jwt-secret-at-least-32-characters-long",
	issuer: "LineWebHookAPI",
	audience: "LineWebHookAdmin",
};

function buildApp(): Hono {
	const app = new Hono();
	app.use("*", createAuthGuard(jwtConfig));
	app.get("/protected", (c) => c.json({ userName: c.get("userName") }));
	return app;
}

describe("createAuthGuard", () => {
	it("有効なアクセストークンがあればnext()を呼び、userNameをコンテキストにセットする", async () => {
		const app = buildApp();
		const { token } = await createAccessToken("admin", jwtConfig, 900);

		const res = await app.request("/protected", {
			headers: { Cookie: `${AUTH_COOKIE_NAME}=${token}` },
		});

		expect(res.status).toBe(200);
		const body = await res.json();
		expect(body.userName).toBe("admin");
	});

	it("アクセストークンCookieが無ければ401を返しnext()を呼ばない", async () => {
		const app = buildApp();

		const res = await app.request("/protected");

		expect(res.status).toBe(401);
	});

	it("不正なアクセストークンなら401を返す", async () => {
		const app = buildApp();

		const res = await app.request("/protected", {
			headers: { Cookie: `${AUTH_COOKIE_NAME}=invalid-token` },
		});

		expect(res.status).toBe(401);
	});

	it("期限切れのアクセストークンなら401を返す", async () => {
		const app = buildApp();
		const { token } = await createAccessToken("admin", jwtConfig, -60);

		const res = await app.request("/protected", {
			headers: { Cookie: `${AUTH_COOKIE_NAME}=${token}` },
		});

		expect(res.status).toBe(401);
	});
});
