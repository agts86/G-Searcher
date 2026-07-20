import { describe, expect, it } from "vitest";
import { hashToken } from "../src/hash.js";

describe("hashToken", () => {
	it("SHA256の大文字16進数文字列を返す（.NETのConvert.ToHexStringと同じ形式）", () => {
		const result = hashToken("sample-refresh-token");

		expect(result).toMatch(/^[0-9A-F]+$/);
		expect(result).toHaveLength(64);
	});

	it("同じ入力に対して常に同じハッシュ値を返す", () => {
		expect(hashToken("same-token")).toBe(hashToken("same-token"));
	});

	it("異なる入力に対して異なるハッシュ値を返す", () => {
		expect(hashToken("token-a")).not.toBe(hashToken("token-b"));
	});

	it("既知の入力に対して.NET側のSHA256(UTF8)と一致するハッシュ値を返す", () => {
		// "admin" のSHA256 (UTF8) は既知値。Convert.ToHexString相当（大文字hex）
		expect(hashToken("admin")).toBe(
			"8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918",
		);
	});
});
