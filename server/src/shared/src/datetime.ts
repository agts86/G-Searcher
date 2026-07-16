const JST_OFFSET_MS = 9 * 60 * 60 * 1000;

function pad(value: number, length = 2): string {
  return String(value).padStart(length, '0');
}

/**
 * 既存.NET側 Meta.CreatedAt/UpdatedAt のシリアライズ挙動（Asia/Tokyo +09:00オフセット付きISO8601）
 * を再現する。サーバーのシステムタイムゾーンに依存せず、UTC時刻に9時間を加算して計算する。
 */
export function formatAsJstIsoString(date: Date): string {
  const jst = new Date(date.getTime() + JST_OFFSET_MS);

  const year = jst.getUTCFullYear();
  const month = pad(jst.getUTCMonth() + 1);
  const day = pad(jst.getUTCDate());
  const hours = pad(jst.getUTCHours());
  const minutes = pad(jst.getUTCMinutes());
  const seconds = pad(jst.getUTCSeconds());
  const millis = pad(jst.getUTCMilliseconds(), 3);

  return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}.${millis}+09:00`;
}
