export interface Env {
  TARGET_URL: string;
}

export default {
  async scheduled(_event: ScheduledEvent, env: Env, _ctx: ExecutionContext) {
    const res = await fetch(env.TARGET_URL, {
      method: "GET",
      headers: { "User-Agent": "cf-cron-warmup" },
    });

    // 起こすだけなら必須じゃないけど、失敗をログに残すと調査が楽
    if (!res.ok) {
      console.log("warmup failed:", res.status, await res.text());
    }
  },
};
