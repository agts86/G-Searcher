import { describe, expect, it } from "vitest";
import { createAccessToken, AUTH_COOKIE_NAME } from "@api/shared";
import { createManagedRouter } from "../src/managed.routes.js";
import { ManagedService } from "../src/managed.service.js";
import { InMemoryManagedRepository } from "./support/in-memory-managed-repository.js";

const jwtConfig = {
	secret: "test-jwt-secret-at-least-32-characters-long",
	issuer: "LineWebHookAPI",
	audience: "LineWebHookAdmin",
};

async function authCookieHeader(): Promise<string> {
	const { token } = await createAccessToken("admin", jwtConfig, 900);
	return `${AUTH_COOKIE_NAME}=${token}`;
}

function setup(): {
	app: ReturnType<typeof createManagedRouter>;
	repo: InMemoryManagedRepository;
} {
	const repo = new InMemoryManagedRepository();
	const service = new ManagedService(repo);
	const app = createManagedRouter(service, jwtConfig);
	return { app, repo };
}

describe("GET /gourmet/:messageType", () => {
	it("認証済みならlocationログをJST文字列で返す", async () => {
		const { app, repo } = setup();
		repo.gourmetLocationLogs = [
			{
				id: "1",
				lat: 35.5,
				lng: 139.5,
				createdAt: new Date("2026-07-08T14:14:45.890Z"),
				updatedAt: new Date("2026-07-08T14:14:45.890Z"),
			},
		];

		const res = await app.request("/gourmet/location", {
			headers: { Cookie: await authCookieHeader() },
		});

		expect(res.status).toBe(200);
		const body = await res.json();
		expect(body).toEqual([
			{
				id: "1",
				lat: 35.5,
				lng: 139.5,
				createdAt: "2026-07-08T23:14:45.890+09:00",
				updatedAt: "2026-07-08T23:14:45.890+09:00",
			},
		]);
	});

	it("認証済みならtextログを返す", async () => {
		const { app, repo } = setup();
		repo.gourmetWordLogs = [
			{
				id: "1",
				text: "ラーメン",
				createdAt: new Date("2026-07-08T00:00:00.000Z"),
				updatedAt: new Date("2026-07-08T00:00:00.000Z"),
			},
		];

		const res = await app.request("/gourmet/text", {
			headers: { Cookie: await authCookieHeader() },
		});

		expect(res.status).toBe(200);
		const body = (await res.json()) as { text: string }[];
		expect(body[0].text).toBe("ラーメン");
	});

	it("未認証なら401を返す", async () => {
		const { app } = setup();

		const res = await app.request("/gourmet/location");

		expect(res.status).toBe(401);
	});

	it("messageTypeが不正なら400を返す", async () => {
		const { app } = setup();

		const res = await app.request("/gourmet/invalid", {
			headers: { Cookie: await authCookieHeader() },
		});

		expect(res.status).toBe(400);
	});
});

describe("GET /error-log", () => {
	it("認証済みならErrorLogを返す", async () => {
		const { app, repo } = setup();
		repo.errorLogs = [
			{
				id: "1",
				contents: "boom",
				createdAt: new Date("2026-07-08T00:00:00.000Z"),
				updatedAt: new Date("2026-07-08T00:00:00.000Z"),
			},
		];

		const res = await app.request("/error-log", {
			headers: { Cookie: await authCookieHeader() },
		});

		expect(res.status).toBe(200);
		const body = (await res.json()) as { contents: string }[];
		expect(body[0].contents).toBe("boom");
	});

	it("未認証なら401を返す", async () => {
		const { app } = setup();

		const res = await app.request("/error-log");

		expect(res.status).toBe(401);
	});
});

describe("GET /job-log", () => {
	it("認証済みならJobLogを返す", async () => {
		const { app, repo } = setup();
		repo.jobLogs = [
			{
				id: "1",
				isSuccess: true,
				contents: "done",
				info: null,
				createdAt: new Date("2026-07-08T00:00:00.000Z"),
				updatedAt: new Date("2026-07-08T00:00:00.000Z"),
			},
		];

		const res = await app.request("/job-log", {
			headers: { Cookie: await authCookieHeader() },
		});

		expect(res.status).toBe(200);
		const body = (await res.json()) as { isSuccess: boolean }[];
		expect(body[0].isSuccess).toBe(true);
	});

	it("未認証なら401を返す", async () => {
		const { app } = setup();

		const res = await app.request("/job-log");

		expect(res.status).toBe(401);
	});
});
