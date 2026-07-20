const ACCESS_EXPIRES_AT_KEY = "linewebhook.accessTokenExpiresAt";

function isBrowser(): boolean {
	return typeof window !== "undefined";
}

export function setAccessTokenExpiresAt(expiresAt: string): void {
	if (!isBrowser()) return;
	localStorage.setItem(ACCESS_EXPIRES_AT_KEY, expiresAt);
}

export function getAccessTokenExpiresAt(): string | null {
	if (!isBrowser()) return null;
	return localStorage.getItem(ACCESS_EXPIRES_AT_KEY);
}

export function clearAccessTokenExpiresAt(): void {
	if (!isBrowser()) return;
	localStorage.removeItem(ACCESS_EXPIRES_AT_KEY);
}
