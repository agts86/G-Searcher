import { serve } from "@hono/node-server";
import { serveStatic } from "@hono/node-server/serve-static";
import { createApp } from "./app.js";

const port = Number(process.env.PORT ?? "3001");
const app = createApp();

// 既存.NET側 `UseDefaultFiles()+UseStaticFiles()` 相当。SPA fallback（未知パスをindex.htmlへ）は
// .NET側にも実装されていないため、ここでも同じ粒度（ディレクトリ配下のindex.html解決のみ）に留める。
app.use(
	"*",
	serveStatic({
		root: "./wwwroot",
		rewriteRequestPath: (path) => (path.endsWith("/") ? `${path}index.html` : path),
	}),
);

serve({ fetch: app.fetch, port }, (info) => {
	// eslint-disable-next-line no-console
	console.log(`api host listening on http://localhost:${info.port}`);
});
