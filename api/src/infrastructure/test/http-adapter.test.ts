import { describe, expect, it, vi, afterEach } from 'vitest';
import { HttpAdapter } from '../src/http-adapter.js';

describe('HttpAdapter.get', () => {
  afterEach(() => {
    vi.unstubAllGlobals();
  });

  it('成功時はJSONをデシリアライズして返す', async () => {
    const fetchMock = vi.fn().mockResolvedValue(
      new Response(JSON.stringify({ hello: 'world' }), { status: 200 }),
    );
    vi.stubGlobal('fetch', fetchMock);
    const adapter = new HttpAdapter();

    const result = await adapter.get<{ hello: string }>('https://example.com/api');

    expect(result).toEqual({ hello: 'world' });
    expect(fetchMock).toHaveBeenCalledWith('https://example.com/api', expect.objectContaining({ method: 'GET' }));
  });

  it('失敗時はステータスコードとレスポンス内容を含むエラーを投げる', async () => {
    const fetchMock = vi.fn().mockResolvedValue(new Response('boom', { status: 500, statusText: 'Internal Server Error' }));
    vi.stubGlobal('fetch', fetchMock);
    const adapter = new HttpAdapter();

    await expect(adapter.get('https://example.com/api')).rejects.toThrow(/500/);
  });

  it('headersを指定できる', async () => {
    const fetchMock = vi.fn().mockResolvedValue(new Response('{}', { status: 200 }));
    vi.stubGlobal('fetch', fetchMock);
    const adapter = new HttpAdapter();

    await adapter.get('https://example.com/api', { Authorization: 'Bearer token' });

    expect(fetchMock).toHaveBeenCalledWith(
      'https://example.com/api',
      expect.objectContaining({ headers: { Authorization: 'Bearer token' } }),
    );
  });
});

describe('HttpAdapter.post', () => {
  afterEach(() => {
    vi.unstubAllGlobals();
  });

  it('bodyをJSON文字列化してContent-Typeを付与する', async () => {
    const fetchMock = vi.fn().mockResolvedValue(new Response(JSON.stringify({ id: '1' }), { status: 201 }));
    vi.stubGlobal('fetch', fetchMock);
    const adapter = new HttpAdapter();

    const result = await adapter.post<{ id: string }, { name: string }>('https://example.com/api', { name: 'test' });

    expect(result).toEqual({ id: '1' });
    expect(fetchMock).toHaveBeenCalledWith(
      'https://example.com/api',
      expect.objectContaining({
        method: 'POST',
        body: JSON.stringify({ name: 'test' }),
        headers: { 'Content-Type': 'application/json' },
      }),
    );
  });

  it('失敗時はエラーを投げる', async () => {
    const fetchMock = vi.fn().mockResolvedValue(new Response('bad request', { status: 400 }));
    vi.stubGlobal('fetch', fetchMock);
    const adapter = new HttpAdapter();

    await expect(adapter.post('https://example.com/api', {})).rejects.toThrow(/400/);
  });
});
