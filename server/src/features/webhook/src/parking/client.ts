export type ParkingLocation = { lat: number; lng: number } | { query: string };

export interface ParkingSpot {
  name: string;
  address: string;
  fee: string | null;
  holiday: string | null;
  detailUrl: string;
  lat: number | null;
  lng: number | null;
}

/**
 * jmpsa.or.jp（全国バイク駐車場・駐輪場案内）の現在地検索/文字列検索呼び出しの抽象。
 * 実装（fetch＋HTMLパースベース）は @api/infrastructure が提供する。
 */
export interface ParkingClient {
  search: (location: ParkingLocation) => Promise<ParkingSpot[]>;
}
