import { afterAll, beforeEach, describe, expect, it } from "vitest";
import { PrismaClient } from "@prisma/client";

// .NET側 ISaveChanges (SaveChangesAdd/SaveChangesModify) と同じ挙動を
// Prismaのスキーマ宣言（@default(now()) / @updatedAt）が再現しているかを実DBで検証する。
const prisma = new PrismaClient();

function sampleToken(
	overrides: Partial<{
		userName: string;
		tokenHash: string;
		expiresAt: Date;
	}> = {},
) {
	return {
		userName: "admin",
		tokenHash: `test-hash-${crypto.randomUUID()}`,
		expiresAt: new Date(Date.now() + 60_000),
		...overrides,
	};
}

beforeEach(async () => {
	await prisma.refreshToken.deleteMany({ where: { userName: "admin" } });
});

afterAll(async () => {
	await prisma.refreshToken.deleteMany({ where: { userName: "admin" } });
	await prisma.$disconnect();
});

describe("RefreshToken作成時 (SaveChangesAdd相当)", () => {
	it("createdAtとupdatedAtが両方とも現在時刻で設定される", async () => {
		const before = new Date();
		const row = await prisma.refreshToken.create({ data: sampleToken() });
		const after = new Date();

		expect(row.createdAt.getTime()).toBeGreaterThanOrEqual(before.getTime());
		expect(row.createdAt.getTime()).toBeLessThanOrEqual(after.getTime());
		expect(row.updatedAt.getTime()).toBe(row.createdAt.getTime());
	});
});

describe("RefreshToken更新時 (SaveChangesModify相当)", () => {
	it("updatedAtのみ更新され、createdAtは変化しない", async () => {
		const created = await prisma.refreshToken.create({ data: sampleToken() });
		const originalCreatedAt = created.createdAt.getTime();

		// createdAtとの差を確実に検出するため少し待つ
		await new Promise((resolve) => setTimeout(resolve, 1100));

		const newExpiresAt = new Date(Date.now() + 120_000);
		const updated = await prisma.refreshToken.update({
			where: { id: created.id },
			data: { expiresAt: newExpiresAt },
		});

		expect(updated.createdAt.getTime()).toBe(originalCreatedAt);
		expect(updated.updatedAt.getTime()).toBeGreaterThan(originalCreatedAt);
		expect(updated.expiresAt.getTime()).toBe(newExpiresAt.getTime());
	});
});
