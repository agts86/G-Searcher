export type SpotLocation = { lat: number; lon: number } | { query: string };

export interface SpotSearchQuery {
  genreCode: string;
  location: SpotLocation;
}

export interface SpotFeature {
  gid: string;
  name: string;
  address: string | null;
  detailUrl: string | null;
}

/**
 * 周辺スポット検索（現状はYahoo!ローカルサーチAPI=YOLP）呼び出しの抽象。公式Node SDKが存在しないため、
 * 実装（fetchベース）は @api/infrastructure が提供する。
 */
export interface SpotSearchClient {
  search: (query: SpotSearchQuery) => Promise<SpotFeature[]>;
}
