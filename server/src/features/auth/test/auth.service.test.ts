import { describe, expect, it, beforeEach } from "vitest";
import { AuthService, UnauthorizedError } from "../src/auth.service.js";
import { InMemoryAuthRepository } from "./support/in-memory-auth-repository.js";

const config = {
	adminUserName: "admin",
	adminPassword: "admin",
	jwt: {
		secret: "test-jwt-secret-at-least-32-characters-long",
		issuer: "LineWebHookAPI",
		audience: "LineWebHookAdmin",
	},
	accessTokenExpiresInSeconds: 900,
	refreshTokenExpiresInSeconds: 604800,
};

describe("AuthService.login", () => {
	let repo: InMemoryAuthRepository;
	let service: AuthService;

	beforeEach(() => {
		repo = new InMemoryAuthRepository();
		service = new AuthService(repo, config);
	});

	it("正しい認証情報でトークンを発行する", async () => {
		const result = await service.login("admin", "admin");

		expect(result.userName).toBe("admin");
		expect(result.accessToken).toBeTruthy();
		expect(result.refreshToken).toBeTruthy();
		expect(repo.size()).toBe(1);
	});

	it('誤ったパスワードは"Invalid user name or password."で失敗する', async () => {
		await expect(service.login("admin", "wrong")).rejects.toThrow(UnauthorizedError);
		await expect(service.login("admin", "wrong")).rejects.toThrow("Invalid user name or password.");
	});

	it("ログイン時に同一ユーザーの既存RefreshTokenを全削除してから新規発行する", async () => {
		await service.login("admin", "admin");
		await service.login("admin", "admin");

		expect(repo.size()).toBe(1);
	});
});

describe("AuthService.refresh", () => {
	let repo: InMemoryAuthRepository;
	let service: AuthService;

	beforeEach(() => {
		repo = new InMemoryAuthRepository();
		service = new AuthService(repo, config);
	});

	it("有効なリフレッシュトークンで新しいトークンをローテーション発行する", async () => {
		const loginResult = await service.login("admin", "admin");

		const refreshed = await service.refresh(loginResult.refreshToken);

		expect(refreshed.userName).toBe("admin");
		expect(refreshed.refreshToken).not.toBe(loginResult.refreshToken);
		expect(repo.size()).toBe(1);
	});

	it('リフレッシュトークンが未指定なら"Unauthorized."で失敗する', async () => {
		await expect(service.refresh(undefined)).rejects.toThrow("Unauthorized.");
	});

	it('存在しないリフレッシュトークンは"Unauthorized."で失敗する', async () => {
		await expect(service.refresh("does-not-exist")).rejects.toThrow("Unauthorized.");
	});

	it('期限切れのリフレッシュトークンは該当行を削除して"Unauthorized."で失敗する', async () => {
		const loginResult = await service.login("admin", "admin");
		const expiredConfig = { ...config, refreshTokenExpiresInSeconds: -1 };
		const expiredService = new AuthService(repo, expiredConfig);
		const expiredLogin = await expiredService.login("admin", "admin");

		await expect(service.refresh(expiredLogin.refreshToken)).rejects.toThrow("Unauthorized.");
		expect(repo.size()).toBe(0);
	});
});

describe("AuthService.logout", () => {
	let repo: InMemoryAuthRepository;
	let service: AuthService;

	beforeEach(() => {
		repo = new InMemoryAuthRepository();
		service = new AuthService(repo, config);
	});

	it("有効なリフレッシュトークンを無効化する", async () => {
		const loginResult = await service.login("admin", "admin");

		await service.logout(loginResult.refreshToken);

		expect(repo.size()).toBe(0);
	});

	it("トークンが存在しなくてもエラーにしない（冪等）", async () => {
		await expect(service.logout("does-not-exist")).resolves.not.toThrow();
		await expect(service.logout(undefined)).resolves.not.toThrow();
	});
});
