import { describe, expect, it } from "vitest";
import * as jose from "jose";
import { createAccessToken, verifyAccessToken, NAME_CLAIM_TYPE } from "../src/jwt.js";

const config = {
	secret: "test-jwt-secret-at-least-32-characters-long",
	issuer: "LineWebHookAPI",
	audience: "LineWebHookAdmin",
};

describe("NAME_CLAIM_TYPE", () => {
	it(".NETのClaimTypes.Nameと同じURI文字列である", () => {
		expect(NAME_CLAIM_TYPE).toBe("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
	});
});

describe("createAccessToken", () => {
	it("既存.NET側と同じクレーム構成のJWTを発行する", async () => {
		const { token, expiresAt } = await createAccessToken("admin", config, 900);

		const secretKey = new TextEncoder().encode(config.secret);
		const { payload, protectedHeader } = await jose.jwtVerify(token, secretKey, {
			issuer: config.issuer,
			audience: config.audience,
		});

		expect(protectedHeader.alg).toBe("HS256");
		expect(payload.sub).toBe("admin");
		expect(payload[NAME_CLAIM_TYPE]).toBe("admin");
		expect(typeof payload.jti).toBe("string");
		expect(payload.iss).toBe(config.issuer);
		expect(payload.aud).toBe(config.audience);
		expect(payload.exp).toBeDefined();
		// JWTのexpは秒単位に切り捨てられるため、最大1秒未満のズレは正常
		expect(Math.floor(expiresAt.getTime() / 1000)).toBe(payload.exp as number);
	});

	it("jtiは呼び出しごとに異なる値になる", async () => {
		const first = await createAccessToken("admin", config, 900);
		const second = await createAccessToken("admin", config, 900);

		const secretKey = new TextEncoder().encode(config.secret);
		const { payload: p1 } = await jose.jwtVerify(first.token, secretKey, config);
		const { payload: p2 } = await jose.jwtVerify(second.token, secretKey, config);

		expect(p1.jti).not.toBe(p2.jti);
	});
});

describe("verifyAccessToken", () => {
	it("createAccessTokenで発行したトークンを検証してユーザー名を返す", async () => {
		const { token } = await createAccessToken("admin", config, 900);

		const result = await verifyAccessToken(token, config);

		expect(result.userName).toBe("admin");
	});

	it("署名が不正なトークンは検証に失敗する", async () => {
		const { token } = await createAccessToken("admin", config, 900);
		const tampered = token.slice(0, -2) + "xx";

		await expect(verifyAccessToken(tampered, config)).rejects.toThrow();
	});

	it("期限切れのトークンは検証に失敗する", async () => {
		const { token } = await createAccessToken("admin", config, -60);

		await expect(verifyAccessToken(token, config)).rejects.toThrow();
	});

	it("issuer/audienceが異なるトークンは検証に失敗する", async () => {
		const { token } = await createAccessToken("admin", config, 900);

		await expect(
			verifyAccessToken(token, { ...config, audience: "OtherAudience" }),
		).rejects.toThrow();
	});
});
