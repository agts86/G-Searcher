import type { HttpAdapter } from './http-adapter.js';
import type { SpotSearchClient, SpotSearchQuery, SpotFeature } from '@api/features-webhook';

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
    Tel1?: string;
    Detail?: {
      PcUrl1?: string;
      MobileUrl1?: string;
      ReviewUrl?: string;
      YUrl?: string;
    };
  };
}

interface YolpLocalSearchResponse {
  Feature?: YolpFeatureResponse[];
}

/**
 * 既存.NET側 DetailInfo.Extra ([JsonExtensionData])は「名前の一致しないJSONフィールドを
 * 自動的に集めるC#側のデシリアライズ機構」であり、実際のレスポンスに"Extra"という入れ子
 * オブジェクトが存在するわけではない。実データで確認したところ、YUrlはDetail直下の
 * フラットなプロパティ（PcUrl1等と同階層）だった。
 */
function resolveDetailUrl(property: YolpFeatureResponse['Property']): string | null {
  const detail = property?.Detail;
  const candidates = [
    detail?.YUrl,
    detail?.PcUrl1,
    detail?.MobileUrl1,
    detail?.ReviewUrl,
    property?.Tel1 ? `tel:${property.Tel1.replace(/-/g, '')}` : undefined,
  ];
  return candidates.find((candidate) => Boolean(candidate)) ?? null;
}

function toYolpFeature(feature: YolpFeatureResponse): SpotFeature {
  const property = feature.Property;
  return {
    gid: feature.Gid ?? feature.Id ?? '',
    name: feature.Name ?? '',
    address: property?.Address ?? null,
    detailUrl: resolveDetailUrl(property),
  };
}

function buildQueryParams(appId: string, query: SpotSearchQuery): URLSearchParams {
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
export class YolpClientImpl implements SpotSearchClient {
  constructor(
    private readonly httpAdapter: HttpAdapter,
    private readonly appId: string,
  ) {}

  async search(query: SpotSearchQuery): Promise<SpotFeature[]> {
    const params = buildQueryParams(this.appId, query);
    const url = `${YOLP_BASE_URL}/search/local/V1/localSearch?${params.toString()}`;

    const response = await this.httpAdapter.get<YolpLocalSearchResponse>(url);
    return (response.Feature ?? []).map(toYolpFeature);
  }
}
