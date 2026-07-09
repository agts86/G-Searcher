import { describe, expect, it, vi } from 'vitest';
import type { Mock } from 'vitest';
import type { HttpAdapter } from '../src/http-adapter.js';
import { YolpClientImpl } from '../src/yolp-client.impl.js';

type GetMock = Mock<(url: string, headers?: Record<string, string>) => Promise<unknown>>;

function buildAdapter(response: unknown): { adapter: HttpAdapter; getMock: GetMock } {
  const getMock: GetMock = vi.fn().mockResolvedValue(response);
  return { adapter: { get: getMock } as unknown as HttpAdapter, getMock };
}

describe('YolpClientImpl.searchLocal', () => {
  it('緯度経度指定の場合はlat/lonクエリでYOLPを呼び、結果をYolpFeatureへ変換する', async () => {
    const { adapter, getMock } = buildAdapter({
      Feature: [
        {
          Gid: 'g1',
          Name: '店A',
          Property: { Address: '東京都千代田区1-1', Detail: { Extra: { YUrl: 'https://example.com/g1' } } },
        },
      ],
    });
    const client = new YolpClientImpl(adapter, 'app-id-1');

    const result = await client.searchLocal({ genreCode: 'genre1', location: { lat: 35.5, lon: 139.5 } });

    expect(result).toEqual([{ gid: 'g1', name: '店A', address: '東京都千代田区1-1', detailUrl: 'https://example.com/g1' }]);
    const [url] = getMock.mock.calls[0];
    expect(url).toContain('lat=35.5');
    expect(url).toContain('lon=139.5');
    expect(url).toContain('gc=genre1');
    expect(url).toContain('appid=app-id-1');
  });

  it('自由文検索の場合はqueryクエリでYOLPを呼ぶ', async () => {
    const { adapter, getMock } = buildAdapter({ Feature: [] });
    const client = new YolpClientImpl(adapter, 'app-id-1');

    await client.searchLocal({ genreCode: 'genre1', location: { query: 'ラーメン うどん' } });

    const [url] = getMock.mock.calls[0];
    expect(url).toContain(new URLSearchParams({ query: 'ラーメン うどん' }).toString());
  });

  it('Featureが無ければ空配列を返す', async () => {
    const { adapter } = buildAdapter({});
    const client = new YolpClientImpl(adapter, 'app-id-1');

    const result = await client.searchLocal({ genreCode: 'genre1', location: { query: 'ラーメン' } });

    expect(result).toEqual([]);
  });

  it('Extraのキーが大文字小文字違い(yurl)でもdetailUrlを拾う（既存.NET側のOrdinalIgnoreCase相当）', async () => {
    const { adapter } = buildAdapter({
      Feature: [{ Gid: 'g1', Name: '店A', Property: { Detail: { Extra: { yurl: 'https://example.com/g1' } } } }],
    });
    const client = new YolpClientImpl(adapter, 'app-id-1');

    const result = await client.searchLocal({ genreCode: 'genre1', location: { query: '店A' } });

    expect(result).toEqual([{ gid: 'g1', name: '店A', address: null, detailUrl: 'https://example.com/g1' }]);
  });

  it('Gid/Address/YUrlが無い場合はId/nullでフォールバックする', async () => {
    const { adapter } = buildAdapter({ Feature: [{ Id: 'id-1', Name: '店B' }] });
    const client = new YolpClientImpl(adapter, 'app-id-1');

    const result = await client.searchLocal({ genreCode: 'genre1', location: { query: '店B' } });

    expect(result).toEqual([{ gid: 'id-1', name: '店B', address: null, detailUrl: null }]);
  });
});
