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

function toYolpFeature(feature: YolpFeatureResponse): YolpFeature {
  return {
    gid: feature.Gid ?? feature.Id ?? '',
    name: feature.Name ?? '',
    address: feature.Property?.Address ?? null,
    detailUrl: findExtraValueCaseInsensitive(feature.Property?.Detail?.Extra, 'YUrl'),
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
