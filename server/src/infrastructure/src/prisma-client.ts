import { PrismaClient } from "@prisma/client";

let client: PrismaClient | undefined;

/** PrismaClientシングルトン。このファイル以外で `new PrismaClient()` しない */
export function getPrismaClient(): PrismaClient {
	if (!client) {
		client = new PrismaClient();
	}
	return client;
}
