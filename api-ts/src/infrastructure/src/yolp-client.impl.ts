import type { HttpAdapter } from './http-adapter.js';
import type { YolpClient, YolpSearchQuery, YolpFeature } from '@api-ts/features-webhook';

const YOLP_BASE_URL = 'https://map.yahooapis.jp';
const RESULTS = 20;
const DIST = 1;
const DETAIL = 'full';

interface YolpFeatureResponse {
  Gid?: string;
  Id?: string;
  Name?: string;
  Property?: {
    Address?: string;
    Detail?: {
      PcUrl1?: string;
      MobileUrl1?: string;
      ReviewUrl?: string;
      Extra?: Record<string, string>;
    };
  };
}

interface YolpLocalSearchResponse {
  Feature?: YolpFeatureResponse[];
}

/**
 * 既存.NET側 DetailInfo.Extra は StringComparer.OrdinalIgnoreCase の辞書（かつ
 * デシリアライズ全体が PropertyNameCaseInsensitive）で "YUrl" を検索していたため、
 * 実際のYOLPレスポンスのキーの大文字小文字が厳密一致しなくても拾えていた。
 * ここでも同じ大文字小文字非依存の挙動に合わせる。
 */
function findExtraValueCaseInsensitive(extra: Record<string, string> | undefined, key: string): string | null {
  if (!extra) {
    return null;
  }
  const lowerKey = key.toLowerCase();
  const foundKey = Object.keys(extra).find((k) => k.toLowerCase() === lowerKey);
  return foundKey ? extra[foundKey] : null;
}

/**
 * "YUrl" はYOLPの公式ドキュメントに存在しないcassette固有の拡張フィールドで、
 * 多くの結果で欠落する（存在しない場合キー自体が返らない仕様）。詳細リンクが
 * 表示されない結果が多発しないよう、公式フィールドの PcUrl1 / MobileUrl1 / ReviewUrl
 * へフォールバックする。
 */
function resolveDetailUrl(property: YolpFeatureResponse['Property']): string | null {
  const detail = property?.Detail;
  const candidates = [
    findExtraValueCaseInsensitive(detail?.Extra, 'YUrl'),
    detail?.PcUrl1,
    detail?.MobileUrl1,
    detail?.ReviewUrl,
  ];
  return candidates.find((candidate) => Boolean(candidate)) ?? null;
}

function toYolpFeature(feature: YolpFeatureResponse): YolpFeature {
  const property = feature.Property;
  return {
    gid: feature.Gid ?? feature.Id ?? '',
    name: feature.Name ?? '',
    address: property?.Address ?? null,
    detailUrl: resolveDetailUrl(property),
  };
}

function buildQueryParams(appId: string, query: YolpSearchQuery): URLSearchParams {
  const params = new URLSearchParams({
    appid: appId,
    output: 'json',
    dist: String(DIST),
    results: String(RESULTS),
    detail: DETAIL,
  });
  if (query.genreCode) {
    params.set('gc', query.genreCode);
  }
  if ('lat' in query.location) {
    params.set('lat', String(query.location.lat));
    params.set('lon', String(query.location.lon));
  } else {
    params.set('query', query.location.query);
  }
  return params;
}

/** Yahoo!ローカルサーチAPI(YOLP)のfetchベース実装。既存.NET側 YOLPClient.GetLocalSearchResultAsync と同じクエリを送る */
export class YolpClientImpl implements YolpClient {
  constructor(
    private readonly httpAdapter: HttpAdapter,
    private readonly appId: string,
  ) {}

  async searchLocal(query: YolpSearchQuery): Promise<YolpFeature[]> {
    const params = buildQueryParams(this.appId, query);
    const url = `${YOLP_BASE_URL}/search/local/V1/localSearch?${params.toString()}`;

    const response = await this.httpAdapter.get<YolpLocalSearchResponse>(url);
    return (response.Feature ?? []).map(toYolpFeature);
  }
}
