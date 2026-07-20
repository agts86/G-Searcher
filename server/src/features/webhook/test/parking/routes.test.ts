import { describe, expect, it, vi } from "vitest";
import { createHmac } from "node:crypto";
import { createParkingRouter } from "../../src/parking/routes.js";
import type { ParkingReplyService } from "../../src/parking/reply.service.js";

const CHANNEL_SECRET = "test-channel-secret";

function signBody(body: string): string {
	return createHmac("sha256", CHANNEL_SECRET).update(body).digest("base64");
}

function buildRequestInit(body: object): RequestInit {
	const json = JSON.stringify(body);
	return {
		method: "POST",
		headers: {
			"Content-Type": "application/json",
			"x-line-signature": signBody(json),
		},
		body: json,
	};
}

function setup(processEventsResult: unknown[] = []): {
	app: ReturnType<typeof createParkingRouter>;
} {
	const service = {
		processEvents: vi.fn().mockResolvedValue(processEventsResult),
	} as unknown as ParkingReplyService;
	const app = createParkingRouter(service, CHANNEL_SECRET, true);
	return { app };
}

describe("POST /parking", () => {
	it("署名が正しければ同期処理し200と結果配列を返す", async () => {
		const { app } = setup([{ reply: { replyToken: "r1", messages: [] }, isReplySucceeded: true }]);

		const res = await app.request("/parking", buildRequestInit({ destination: "U1", events: [] }));

		expect(res.status).toBe(200);
		const body = await res.json();
		expect(body).toEqual([{ reply: { replyToken: "r1", messages: [] }, isReplySucceeded: true }]);
	});

	it("署名が無ければ401を返す", async () => {
		const { app } = setup();

		const res = await app.request("/parking", {
			method: "POST",
			headers: { "Content-Type": "application/json" },
			body: JSON.stringify({ destination: "U1", events: [] }),
		});

		expect(res.status).toBe(401);
	});
});
