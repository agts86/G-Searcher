import * as cheerio from "cheerio";
import type { Element } from "domhandler";
import type { HttpAdapter } from "./http-adapter.js";
import type { ParkingClient, ParkingLocation, ParkingSpot } from "@api/features-webhook";

const JMPSA_BASE_URL = "https://www.jmpsa.or.jp/society/parking";
const LIST_ITEM_SELECTOR = ".p-parking-prefecture-list-item";
// 「駐車可能車両を変更」「予約制の駐車場を除く」の絞り込みは、location.php/search.phpの
// URLクエリではなくCookieの`vs`値でサーバー側に伝わる（`qr[]`クエリは実際には無視される、実機検証済み）。
// 50cc/51-125cc/126cc以上は対象、記載なしは対象外、予約制の駐車場は除外、で固定する。
const VS_COOKIE = "vs=1,1,1,0,1";

function buildUrl(location: ParkingLocation): string {
	if ("lat" in location) {
		const params = new URLSearchParams({
			lng: String(location.lng),
			lat: String(location.lat),
		});
		return `${JMPSA_BASE_URL}/location.php?${params.toString()}`;
	}

	const params = new URLSearchParams({
		q: location.query,
		p_pref: "",
		p_sect: "",
	});
	return `${JMPSA_BASE_URL}/search.php?${params.toString()}`;
}

/** Google Maps embed iframeのsrc（`q=<lat>,<lng>`）から座標を取り出す。iframeが無い項目はnullにする */
function parseCoordinates(iframeSrc: string): {
	lat: number | null;
	lng: number | null;
} {
	const match = /[?&]q=(-?[\d.]+),(-?[\d.]+)/.exec(iframeSrc);
	if (!match) {
		return { lat: null, lng: null };
	}
	return { lat: Number(match[1]), lng: Number(match[2]) };
}

function findTableValue($: cheerio.CheerioAPI, item: Element, label: string): string | null {
	let value: string | null = null;
	$(item)
		.find(".p-parking-prefecture-table-txt-box")
		.each((_, box) => {
			const $box = $(box);
			if ($box.find(".p-parking-prefecture-table-ttl").text().trim() === label) {
				value = $box.find(".p-parking-prefecture-table-txt").text().trim();
			}
		});
	return value;
}

function toSpot($: cheerio.CheerioAPI, item: Element): ParkingSpot {
	const $item = $(item);
	const anchor = $item.find(".p-parking-prefecture-map-ttl a");
	anchor.find(".m-arrow").remove();
	const iframeSrc = $item.find(".p-parking-prefecture-map-iframe iframe").attr("src") ?? "";

	return {
		name: anchor.text().trim(),
		detailUrl: anchor.attr("href") ?? "",
		address: $item.find(".p-parking-prefecture-map-txt").text().trim(),
		holiday: findTableValue($, item, "定休日"),
		fee: findTableValue($, item, "料金"),
		...parseCoordinates(iframeSrc),
	};
}

/**
 * jmpsa.or.jp（全国バイク駐車場・駐輪場案内）のfetch＋HTMLパースベース実装。
 * location.php/search.phpはHTML断片ではなくページ全体を返すが、駐車場一覧部分
 * （`.p-parking-prefecture-list-item`）だけをcheerioで抽出する。
 */
export class ParkingClientImpl implements ParkingClient {
	constructor(private readonly httpAdapter: HttpAdapter) {}

	async search(location: ParkingLocation): Promise<ParkingSpot[]> {
		const html = await this.httpAdapter.getText(buildUrl(location), {
			Cookie: VS_COOKIE,
		});
		const $ = cheerio.load(html);
		return $(LIST_ITEM_SELECTOR)
			.map((_, item) => toSpot($, item))
			.get();
	}
}
