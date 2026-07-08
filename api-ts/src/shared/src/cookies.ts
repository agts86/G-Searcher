export const AUTH_COOKIE_NAME = 'linewebhook_auth';
export const AUTH_REFRESH_COOKIE_NAME = 'linewebhook_refresh';

export interface AuthCookieOptions {
  httpOnly: true;
  secure: true;
  sameSite: 'Strict';
  path: '/';
  expires: Date;
}

/** 既存.NET側 AuthCookieOptionsFactory.Create と同じCookie属性を生成する */
export function buildAuthCookieOptions(expiresAt: Date): AuthCookieOptions {
  return {
    httpOnly: true,
    secure: true,
    sameSite: 'Strict',
    path: '/',
    expires: expiresAt,
  };
}
