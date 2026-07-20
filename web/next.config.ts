import type { NextConfig } from "next";

const isDev = process.env.NODE_ENV === "development";
const apiBaseUrl = (process.env.API_BASE_URL ?? "http://localhost:3001").replace(/\/$/, "");

const nextConfig: NextConfig = {
	output: isDev ? undefined : "export",
	trailingSlash: true,
	images: {
		unoptimized: true,
	},
	// dev 時のみ API プロキシを有効化（本番は同一オリジンのため不要）
	...(isDev && {
		rewrites: async () => [
			{
				source: "/api/:path*",
				destination: `${apiBaseUrl}/api/:path*`,
			},
		],
	}),
};

export default nextConfig;
