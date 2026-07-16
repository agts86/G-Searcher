/**
 * fetchベースの汎用HTTPアダプタ。既存.NET側 LineDevSdk.Http.HttpAdapter と同じ考え方
 * （Get/Post/Put/Delete＋失敗時はステータスコードとレスポンス内容を含むエラーを投げる）。
 * axiosなど外部HTTPクライアントライブラリは使わず、Node組み込みのfetchのみを使う。
 */
export class HttpAdapter {
  async get<T>(url: string, headers?: Record<string, string>): Promise<T> {
    return this.send<T>(url, { method: 'GET', headers }, (res) => res.json() as Promise<T>);
  }

  /** JSONではなくHTML/プレーンテキストを返すエンドポイント向け（外部サイトのHTML断片取得等） */
  async getText(url: string, headers?: Record<string, string>): Promise<string> {
    return this.send<string>(url, { method: 'GET', headers }, (res) => res.text());
  }

  async post<T, TBody>(url: string, body: TBody, headers?: Record<string, string>): Promise<T> {
    return this.send<T>(
      url,
      {
        method: 'POST',
        headers: { ...headers, 'Content-Type': 'application/json' },
        body: JSON.stringify(body),
      },
      (res) => res.json() as Promise<T>,
    );
  }

  async put<T, TBody>(url: string, body: TBody, headers?: Record<string, string>): Promise<T> {
    return this.send<T>(
      url,
      {
        method: 'PUT',
        headers: { ...headers, 'Content-Type': 'application/json' },
        body: JSON.stringify(body),
      },
      (res) => res.json() as Promise<T>,
    );
  }

  async delete<T>(url: string, headers?: Record<string, string>): Promise<T> {
    return this.send<T>(url, { method: 'DELETE', headers }, (res) => res.json() as Promise<T>);
  }

  private async send<T>(url: string, init: RequestInit, parse: (res: Response) => Promise<T>): Promise<T> {
    const res = await fetch(url, init);
    if (!res.ok) {
      const errorBody = await res.text();
      throw new Error(
        `Request failed: ${res.status} ${res.statusText}\nRequest: ${init.method ?? 'GET'} ${url}\nResponse: ${errorBody}`,
      );
    }
    return parse(res);
  }
}
