/** GourmetLocationLog */
export type GourmetLocationLog = {
	id: string;
	lat: number;
	lng: number;
	createdAt: string;
	updatedAt: string;
};

/** GourmetWordLog */
export type GourmetWordLog = {
	id: string;
	text: string;
	createdAt: string;
	updatedAt: string;
};

/** ErrorLog */
export type ErrorLog = {
	id: string;
	contents: string;
	createdAt: string;
	updatedAt: string;
};

/** JobLog */
export type JobLog = {
	id: string;
	isSuccess: boolean;
	contents: string | null;
	info: string | null;
	createdAt: string;
	updatedAt: string;
};

/** ログインリクエスト */
export type LoginRequest = {
	userName: string;
	password: string;
};

/** ログインレスポンス */
export type LoginResponse = {
	userName: string;
	expiresAt: string;
};

/** /auth/me レスポンス */
export type MeResponse = {
	userName: string;
};
