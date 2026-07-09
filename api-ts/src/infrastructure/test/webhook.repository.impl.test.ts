import { afterAll, beforeEach, describe, expect, it } from 'vitest';
import { randomUUID } from 'node:crypto';
import { PrismaClient } from '@prisma/client';
import { PrismaWebhookRepository } from '../src/webhook.repository.impl.js';

const prisma = new PrismaClient();
const repo = new PrismaWebhookRepository(prisma);

beforeEach(async () => {
  await prisma.gourmetLocationLog.deleteMany();
  await prisma.gourmetWordLog.deleteMany();
  await prisma.jobLog.deleteMany();
});

afterAll(async () => {
  await prisma.gourmetLocationLog.deleteMany();
  await prisma.gourmetWordLog.deleteMany();
  await prisma.jobLog.deleteMany();
  await prisma.$disconnect();
});

describe('PrismaWebhookRepository.createGourmetLocationLog', () => {
  it('渡されたid/lat/lng/createdAt/updatedAtをそのまま保存する', async () => {
    const id = randomUUID();
    const createdAt = '2026-01-01T00:00:00.000+09:00';

    await repo.createGourmetLocationLog({ id, lat: 35.5, lng: 139.5, createdAt, updatedAt: createdAt });

    const rows = await prisma.gourmetLocationLog.findMany();
    expect(rows).toHaveLength(1);
    expect(rows[0]).toMatchObject({ id, lat: 35.5, lng: 139.5, createdAt: new Date(createdAt), updatedAt: new Date(createdAt) });
  });
});

describe('PrismaWebhookRepository.createGourmetWordLog', () => {
  it('渡されたid/text/createdAt/updatedAtをそのまま保存する', async () => {
    const id = randomUUID();
    const createdAt = '2026-01-01T00:00:00.000+09:00';

    await repo.createGourmetWordLog({ id, text: 'ラーメン', createdAt, updatedAt: createdAt });

    const rows = await prisma.gourmetWordLog.findMany();
    expect(rows).toHaveLength(1);
    expect(rows[0]).toMatchObject({ id, text: 'ラーメン', createdAt: new Date(createdAt), updatedAt: new Date(createdAt) });
  });
});

describe('PrismaWebhookRepository.createJobLog', () => {
  it('id/isSuccess/contents/infoを保存する', async () => {
    const id = randomUUID();

    await repo.createJobLog({ id, isSuccess: true, contents: 'ok', info: null });

    const rows = await prisma.jobLog.findMany();
    expect(rows).toHaveLength(1);
    expect(rows[0]).toMatchObject({ id, isSuccess: true, contents: 'ok', info: null });
  });
});
