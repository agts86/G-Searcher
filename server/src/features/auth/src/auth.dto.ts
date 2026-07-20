import { z } from "@hono/zod-openapi";

export const LoginRequestSchema = z
	.object({
		userName: z.string().min(1).openapi({ example: "admin" }),
		password: z.string().min(1).openapi({ example: "admin" }),
	})
	.openapi("LoginRequest");
export type LoginRequest = z.infer<typeof LoginRequestSchema>;

export const LoginResponseSchema = z
	.object({
		userName: z.string().openapi({ example: "admin" }),
		expiresAt: z.string().openapi({ example: "2026-07-07T19:29:45.890Z" }),
	})
	.openapi("LoginResponse");
export type LoginResponse = z.infer<typeof LoginResponseSchema>;

export const MeResponseSchema = z
	.object({
		userName: z.string().openapi({ example: "admin" }),
	})
	.openapi("MeResponse");
export type MeResponse = z.infer<typeof MeResponseSchema>;

export const ErrorResponseSchema = z
	.object({
		message: z.string().openapi({ example: "Unauthorized." }),
	})
	.openapi("ErrorResponse");
