import { afterAll, beforeEach, describe, expect, it } from "vitest";
import { PrismaClient } from "@prisma/client";
import { PrismaManagedRepository } from "../src/managed.repository.impl.js";

const prisma = new PrismaClient();
const repo = new PrismaManagedRepository(prisma);

beforeEach(async () => {
	await prisma.gourmetLocationLog.deleteMany();
	await prisma.gourmetWordLog.deleteMany();
	await prisma.errorLog.deleteMany();
	await prisma.jobLog.deleteMany();
});

afterAll(async () => {
	await prisma.gourmetLocationLog.deleteMany();
	await prisma.gourmetWordLog.deleteMany();
	await prisma.errorLog.deleteMany();
	await prisma.jobLog.deleteMany();
	await prisma.$disconnect();
});

describe("PrismaManagedRepository.findGourmetLocationLogs", () => {
	it("createdAt降順で全件返す", async () => {
		const older = await prisma.gourmetLocationLog.create({
			data: { lat: 1, lng: 1 },
		});
		await new Promise((resolve) => setTimeout(resolve, 10));
		const newer = await prisma.gourmetLocationLog.create({
			data: { lat: 2, lng: 2 },
		});

		const result = await repo.findGourmetLocationLogs();

		expect(result.map((r) => r.id)).toEqual([newer.id, older.id]);
	});
});

describe("PrismaManagedRepository.findGourmetWordLogs", () => {
	it("createdAt降順で全件返す", async () => {
		const older = await prisma.gourmetWordLog.create({
			data: { text: "カレー" },
		});
		await new Promise((resolve) => setTimeout(resolve, 10));
		const newer = await prisma.gourmetWordLog.create({
			data: { text: "うどん" },
		});

		const result = await repo.findGourmetWordLogs();

		expect(result.map((r) => r.id)).toEqual([newer.id, older.id]);
	});
});

describe("PrismaManagedRepository.findErrorLogs", () => {
	it("createdAt降順で全件返す", async () => {
		const older = await prisma.errorLog.create({ data: { contents: "err-1" } });
		await new Promise((resolve) => setTimeout(resolve, 10));
		const newer = await prisma.errorLog.create({ data: { contents: "err-2" } });

		const result = await repo.findErrorLogs();

		expect(result.map((r) => r.id)).toEqual([newer.id, older.id]);
	});
});

describe("PrismaManagedRepository.findJobLogs", () => {
	it("createdAt降順で全件返す", async () => {
		const older = await prisma.jobLog.create({
			data: { isSuccess: true, contents: "ok", info: null },
		});
		await new Promise((resolve) => setTimeout(resolve, 10));
		const newer = await prisma.jobLog.create({
			data: { isSuccess: false, contents: null, info: "ng" },
		});

		const result = await repo.findJobLogs();

		expect(result.map((r) => r.id)).toEqual([newer.id, older.id]);
	});
});
