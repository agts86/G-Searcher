import { describe, expect, it, vi } from 'vitest';
import type { Mock } from 'vitest';
import type { HttpAdapter } from '../src/http-adapter.js';
import { ParkingClientImpl } from '../src/parking-client.impl.js';

type GetTextMock = Mock<(url: string, headers?: Record<string, string>) => Promise<string>>;

function buildAdapter(html: string): { adapter: HttpAdapter; getTextMock: GetTextMock } {
  const getTextMock: GetTextMock = vi.fn().mockResolvedValue(html);
  return { adapter: { getText: getTextMock } as unknown as HttpAdapter, getTextMock };
}

// 実際にjmpsa.or.jp/assets/module/maplist.phpへPlaywright MCPでリクエストし取得したレスポンスの抜粋（2件分）
const TWO_SPOTS_HTML = `
<li class="p-parking-prefecture-list-item">
	<div class="p-parking-prefecture-wrap">
		<div class="p-parking-prefecture-map">
			<p class="p-parking-prefecture-map-iframe"><iframe frameborder="0" scrolling="no" marginheight="0" marginwidth="0" src="//www.google.com/maps/embed/v1/place?key=AIzaSyC7m01RSCW9r5ViCy4asY3xdYfEp2blP1g&q=35.842247,139.800044&zoom=14"></iframe></p>
		</div>
		<div class="p-parking-prefecture-txt-box">
			<p class="p-parking-prefecture-map-ttl"><a href="/society/parking/area11/p-8005.html" class="m-c-link">エコステーション21\u3000ハーモネスタワー松原団地Bエリア<span class="m-arrow"></span></a></p>
			<p class="p-parking-prefecture-map-txt">草加市松原1-1-6</p>
			<div class="p-parking-prefecture-table-wrap">
				<div class="p-parking-prefecture-table">
					<div class="p-parking-prefecture-table-txt-box">
						<p class="p-parking-prefecture-table-ttl">定休日</p>
						<p class="p-parking-prefecture-table-txt">-</p>
					</div>
					<div class="p-parking-prefecture-table-txt-box">
						<p class="p-parking-prefecture-table-ttl">料金</p>
						<p class="p-parking-prefecture-table-txt">4時間毎200円\u3000※3時間まで無料</p>
					</div>
				</div>
			</div>
		</div>
	</div>
</li>
<li class="p-parking-prefecture-list-item">
	<div class="p-parking-prefecture-wrap">
		<div class="p-parking-prefecture-map">
			<p class="p-parking-prefecture-map-iframe"></p>
		</div>
		<div class="p-parking-prefecture-txt-box">
			<p class="p-parking-prefecture-map-ttl"><a href="/society/parking/area11/p-20771.html" class="m-c-link">新田第1駐輪場<span class="m-arrow"></span></a></p>
			<p class="p-parking-prefecture-map-txt">草加市金明町263-2</p>
			<div class="p-parking-prefecture-table-wrap">
				<div class="p-parking-prefecture-table">
					<div class="p-parking-prefecture-table-txt-box">
						<p class="p-parking-prefecture-table-ttl">定休日</p>
						<p class="p-parking-prefecture-table-txt">なし</p>
					</div>
					<div class="p-parking-prefecture-table-txt-box">
						<p class="p-parking-prefecture-table-ttl">料金</p>
						<p class="p-parking-prefecture-table-txt">原付（125cc以下）300円/当日・1回、中型400円/当日・1回</p>
					</div>
				</div>
			</div>
		</div>
	</div>
</li>
`;

describe('ParkingClientImpl.search', () => {
  it('緯度経度指定の場合はlocation.phpをlng/latで呼び、結果をParkingSpotへ変換する', async () => {
    const { adapter, getTextMock } = buildAdapter(TWO_SPOTS_HTML);
    const client = new ParkingClientImpl(adapter);

    const result = await client.search({ lat: 35.83842406478811, lng: 139.7962472487242 });

    expect(result).toEqual([
      {
        name: 'エコステーション21\u3000ハーモネスタワー松原団地Bエリア',
        address: '草加市松原1-1-6',
        holiday: '-',
        fee: '4時間毎200円\u3000※3時間まで無料',
        detailUrl: '/society/parking/area11/p-8005.html',
        lat: 35.842247,
        lng: 139.800044,
      },
      {
        name: '新田第1駐輪場',
        address: '草加市金明町263-2',
        holiday: 'なし',
        fee: '原付（125cc以下）300円/当日・1回、中型400円/当日・1回',
        detailUrl: '/society/parking/area11/p-20771.html',
        lat: null,
        lng: null,
      },
    ]);
    const [url] = getTextMock.mock.calls[0];
    expect(url).toContain('https://www.jmpsa.or.jp/society/parking/location.php');
    expect(url).toContain('lng=139.7962472487242');
    expect(url).toContain('lat=35.83842406478811');
  });

  it('文字列検索の場合はsearch.phpをqで呼ぶ', async () => {
    const { adapter, getTextMock } = buildAdapter('');
    const client = new ParkingClientImpl(adapter);

    await client.search({ query: 'スカイツリー' });

    const [url] = getTextMock.mock.calls[0];
    expect(url).toContain('https://www.jmpsa.or.jp/society/parking/search.php');
    expect(url).toContain('q=%E3%82%B9%E3%82%AB%E3%82%A4%E3%83%84%E3%83%AA%E3%83%BC');
  });

  it('該当駐車場が無ければ空配列を返す', async () => {
    const { adapter } = buildAdapter('');
    const client = new ParkingClientImpl(adapter);

    const result = await client.search({ query: '該当なし' });

    expect(result).toEqual([]);
  });

  it('車両フィルタ・予約制除くの絞り込みはCookieのvs値でサーバー側に伝わるため、Cookieヘッダーを付与する', async () => {
    const { adapter, getTextMock } = buildAdapter('');
    const client = new ParkingClientImpl(adapter);

    await client.search({ lat: 35.83842406478811, lng: 139.7962472487242 });

    const [, headers] = getTextMock.mock.calls[0];
    expect(headers).toEqual({ Cookie: 'vs=1,1,1,0,1' });
  });
});
