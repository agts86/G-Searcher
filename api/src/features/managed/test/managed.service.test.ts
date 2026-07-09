import { describe, expect, it, beforeEach } from 'vitest';
import { ManagedService } from '../src/managed.service.js';
import { InMemoryManagedRepository } from './support/in-memory-managed-repository.js';

describe('ManagedService.getGourmetLogs', () => {
  let repo: InMemoryManagedRepository;
  let service: ManagedService;

  beforeEach(() => {
    repo = new InMemoryManagedRepository();
    service = new ManagedService(repo);
  });

  it('messageType=locationならGourmetLocationLogを返す', async () => {
    repo.gourmetLocationLogs = [
      { id: '1', lat: 35.0, lng: 139.0, createdAt: new Date('2026-01-01T00:00:00Z'), updatedAt: new Date('2026-01-01T00:00:00Z') },
    ];

    const result = await service.getGourmetLogs('location');

    expect(result).toEqual(repo.gourmetLocationLogs);
  });

  it('messageType=textならGourmetWordLogを返す', async () => {
    repo.gourmetWordLogs = [
      { id: '1', text: 'ラーメン', createdAt: new Date('2026-01-01T00:00:00Z'), updatedAt: new Date('2026-01-01T00:00:00Z') },
    ];

    const result = await service.getGourmetLogs('text');

    expect(result).toEqual(repo.gourmetWordLogs);
  });

  it('createdAt降順（後に追加した方が先）で返す', async () => {
    const older = { id: '1', lat: 1, lng: 1, createdAt: new Date('2026-01-01T00:00:00Z'), updatedAt: new Date('2026-01-01T00:00:00Z') };
    const newer = { id: '2', lat: 2, lng: 2, createdAt: new Date('2026-01-02T00:00:00Z'), updatedAt: new Date('2026-01-02T00:00:00Z') };
    repo.gourmetLocationLogs = [older, newer];

    const result = await service.getGourmetLogs('location');

    expect(result.map((r) => r.id)).toEqual(['2', '1']);
  });
});

describe('ManagedService.getErrorLogs', () => {
  it('ErrorLogをcreatedAt降順で返す', async () => {
    const repo = new InMemoryManagedRepository();
    const older = { id: '1', contents: 'err1', createdAt: new Date('2026-01-01T00:00:00Z'), updatedAt: new Date('2026-01-01T00:00:00Z') };
    const newer = { id: '2', contents: 'err2', createdAt: new Date('2026-01-02T00:00:00Z'), updatedAt: new Date('2026-01-02T00:00:00Z') };
    repo.errorLogs = [older, newer];
    const service = new ManagedService(repo);

    const result = await service.getErrorLogs();

    expect(result.map((r) => r.id)).toEqual(['2', '1']);
  });
});

describe('ManagedService.getJobLogs', () => {
  it('JobLogをcreatedAt降順で返す', async () => {
    const repo = new InMemoryManagedRepository();
    const older = {
      id: '1',
      isSuccess: true,
      contents: 'ok',
      info: null,
      createdAt: new Date('2026-01-01T00:00:00Z'),
      updatedAt: new Date('2026-01-01T00:00:00Z'),
    };
    const newer = {
      id: '2',
      isSuccess: false,
      contents: null,
      info: 'failed',
      createdAt: new Date('2026-01-02T00:00:00Z'),
      updatedAt: new Date('2026-01-02T00:00:00Z'),
    };
    repo.jobLogs = [older, newer];
    const service = new ManagedService(repo);

    const result = await service.getJobLogs();

    expect(result.map((r) => r.id)).toEqual(['2', '1']);
  });
});
