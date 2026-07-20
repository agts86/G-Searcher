import { z } from "@hono/zod-openapi";

export const MessageTypeParamSchema = z.object({
	messageType: z.enum(["location", "text"]).openapi({ param: { name: "messageType", in: "path" } }),
});

export const GourmetLocationLogSchema = z
	.object({
		id: z.string(),
		lat: z.number(),
		lng: z.number(),
		createdAt: z.string(),
		updatedAt: z.string(),
	})
	.openapi("GourmetLocationLog");
export const GourmetLocationLogsResponseSchema = z.array(GourmetLocationLogSchema);

export const GourmetWordLogSchema = z
	.object({
		id: z.string(),
		text: z.string().nullable(),
		createdAt: z.string(),
		updatedAt: z.string(),
	})
	.openapi("GourmetWordLog");
export const GourmetWordLogsResponseSchema = z.array(GourmetWordLogSchema);

export const GourmetLogsResponseSchema = z.union([
	GourmetLocationLogsResponseSchema,
	GourmetWordLogsResponseSchema,
]);

export const ErrorLogSchema = z
	.object({
		id: z.string(),
		contents: z.string(),
		createdAt: z.string(),
		updatedAt: z.string(),
	})
	.openapi("ErrorLog");
export const ErrorLogsResponseSchema = z.array(ErrorLogSchema);

export const JobLogSchema = z
	.object({
		id: z.string(),
		isSuccess: z.boolean(),
		contents: z.string().nullable(),
		info: z.string().nullable(),
		createdAt: z.string(),
		updatedAt: z.string(),
	})
	.openapi("JobLog");
export const JobLogsResponseSchema = z.array(JobLogSchema);
