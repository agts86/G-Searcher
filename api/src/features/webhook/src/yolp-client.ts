export type YolpLocation = { lat: number; lon: number } | { query: string };

export interface YolpSearchQuery {
  genreCode: string;
  location: YolpLocation;
}

export interface YolpFeature {
  gid: string;
  name: string;
  address: string | null;
  detailUrl: string | null;
}

/**
 * Yahoo!ローカルサーチAPI(YOLP)呼び出しの抽象。公式Node SDKが存在しないため、
 * 実装（fetchベース）は @api/infrastructure が提供する。
 */
export interface YolpClient {
  searchLocal: (query: YolpSearchQuery) => Promise<YolpFeature[]>;
}
