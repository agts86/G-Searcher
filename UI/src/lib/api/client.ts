import { setAccessTokenExpiresAt } from '@/lib/auth/session';

/** API 基底 URL。同一オリジン前提のため相対パスで固定 */
const BASE_URL = '/api/v1';
const AUTH_PATHS_WITHOUT_REFRESH = new Set(['/auth/login', '/auth/logout', '/auth/refresh']);

type HttpMethod = 'GET' | 'POST' | 'DELETE';

export class ApiError extends Error {
  constructor(
    public readonly status: number,
    message: string,
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

let refreshInFlight: Promise<void> | null = null;

async function refreshAccessToken(): Promise<void> {
  if (refreshInFlight) return refreshInFlight;

  refreshInFlight = (async (): Promise<void> => {
    const res = await fetch(`${BASE_URL}/auth/refresh`, {
      method: 'POST',
      credentials: 'include',
    });

    if (!res.ok) {
      throw new ApiError(res.status, `POST /auth/refresh failed: ${res.status}`);
    }

    const data = (await res.json()) as { expiresAt?: string };
    if (data.expiresAt) {
      setAccessTokenExpiresAt(data.expiresAt);
    }
  })().finally(() => {
    refreshInFlight = null;
  });

  return refreshInFlight;
}

async function request<T>(method: HttpMethod, path: string, body?: unknown, canRetry = true): Promise<T> {
  const res = await fetch(`${BASE_URL}${path}`, {
    method,
    credentials: 'include',
    headers: body ? { 'Content-Type': 'application/json' } : undefined,
    body: body ? JSON.stringify(body) : undefined,
  });

  if (!res.ok) {
    if (res.status === 401 && canRetry && !AUTH_PATHS_WITHOUT_REFRESH.has(path)) {
      await refreshAccessToken();
      return request<T>(method, path, body, false);
    }
    throw new ApiError(res.status, `${method} ${path} failed: ${res.status}`);
  }

  if (res.status === 204) {
    return undefined as T;
  }

  return res.json() as Promise<T>;
}

export const apiClient = {
  get: <T>(path: string): Promise<T> => request<T>('GET', path),
  post: <T>(path: string, body: unknown): Promise<T> => request<T>('POST', path, body),
  delete: <T>(path: string): Promise<T> => request<T>('DELETE', path),
};
