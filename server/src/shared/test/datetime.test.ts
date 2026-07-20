import { describe, expect, it } from "vitest";
import { formatAsJstIsoString } from "../src/datetime.js";

describe("formatAsJstIsoString", () => {
	it("UTC日時をAsia/Tokyo(+09:00)のISO8601文字列に変換する", () => {
		// UTC 2026-07-08T14:14:45.890Z は JST 2026-07-08T23:14:45.890+09:00
		const date = new Date("2026-07-08T14:14:45.890Z");

		expect(formatAsJstIsoString(date)).toBe("2026-07-08T23:14:45.890+09:00");
	});

	it("日付が変わる境界（UTC深夜〜JST朝）も正しく変換する", () => {
		// UTC 2026-07-08T15:30:00.000Z は JST 2026-07-09T00:30:00.000+09:00（日付が繰り上がる）
		const date = new Date("2026-07-08T15:30:00.000Z");

		expect(formatAsJstIsoString(date)).toBe("2026-07-09T00:30:00.000+09:00");
	});

	it("常に+09:00オフセットを付与する(Zサフィックスにしない)", () => {
		const date = new Date("2026-01-01T00:00:00.000Z");

		expect(formatAsJstIsoString(date)).toMatch(/\+09:00$/);
		expect(formatAsJstIsoString(date)).not.toContain("Z");
	});
});
