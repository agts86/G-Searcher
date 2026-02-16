/**
 * ISO 8601 文字列を "YYYY/MM/DD HH:mm" 形式にフォーマットする
 */
export function formatDateTime(iso: string): string {
  const d = new Date(iso);
  const y = d.getFullYear();
  const mo = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  const h = String(d.getHours()).padStart(2, '0');
  const min = String(d.getMinutes()).padStart(2, '0');
  return `${y}/${mo}/${day} ${h}:${min}`;
}

/**
 * 日付文字列（YYYY-MM-DD）の範囲でフィルタリングする述語を返す
 */
export function isInDateRange(iso: string, from: string, to: string): boolean {
  if (!from && !to) return true;
  const d = new Date(iso);
  if (from && d < new Date(from)) return false;
  if (to && d > new Date(`${to}T23:59:59`)) return false;
  return true;
}
