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
  it('lat/lngを保存する', async () => {
    await repo.createGourmetLocationLog({ lat: 35.5, lng: 139.5 });

    const rows = await prisma.gourmetLocationLog.findMany();
    expect(rows).toHaveLength(1);
    expect(rows[0]).toMatchObject({ lat: 35.5, lng: 139.5 });
  });
});

describe('PrismaWebhookRepository.createGourmetWordLog', () => {
  it('textを保存する', async () => {
    await repo.createGourmetWordLog({ text: 'ラーメン' });

    const rows = await prisma.gourmetWordLog.findMany();
    expect(rows).toHaveLength(1);
    expect(rows[0]).toMatchObject({ text: 'ラーメン' });
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
