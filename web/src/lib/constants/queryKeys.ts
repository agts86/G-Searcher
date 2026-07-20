export const QueryKeys = {
	auth: {
		me: ["auth", "me"] as const,
	},
	managed: {
		gourmetLocation: ["managed", "gourmetLocation"] as const,
		gourmetWord: ["managed", "gourmetWord"] as const,
		errorLog: ["managed", "errorLog"] as const,
		jobLog: ["managed", "jobLog"] as const,
	},
} as const;
