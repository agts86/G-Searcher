import { PGlite } from "@electric-sql/pglite";
import { PGLiteSocketServer } from "@electric-sql/pglite-socket";
import { execFile } from "node:child_process";
import { promisify } from "node:util";
import { connect } from "node:net";
import { fileURLToPath } from "node:url";
import { dirname, resolve } from "node:path";

const execFileAsync = promisify(execFile);

const __dirname = dirname(fileURLToPath(import.meta.url));
const TABLES_ROOT = resolve(__dirname, "..");

const PGLITE_HOST = "127.0.0.1";
// docker-composeの実Postgres(開発用DB、5432番)とは意図的に別ポートにする。
// テストがdeleteMany()等で開発用DBのデータを壊さないよう、常に専用DBを使う
// （「空いていればPGlite、埋まっていれば既存DB」というフォールバックはしない）。
const PGLITE_PORT = 5433;

export const PGLITE_DATABASE_URL = `postgresql://postgres:postgres@${PGLITE_HOST}:${PGLITE_PORT}/postgres?schema=public&sslmode=disable`;

let db: PGlite | null = null;
let server: PGLiteSocketServer | null = null;

async function canConnect(port: number, host: string): Promise<boolean> {
	return new Promise<boolean>((resolvePromise) => {
		const socket = connect(port, host);
		socket.once("connect", () => {
			socket.end();
			resolvePromise(true);
		});
		socket.once("error", () => resolvePromise(false));
	});
}

// server.start()のPromiseはリッスン開始要求の受理を示すのみで、実際にTCP接続を
// 受け付け可能になるまでわずかにラグがある。接続できるまでポーリングして待つ。
async function waitUntilAcceptingConnections(timeoutMs = 5000): Promise<void> {
	const deadline = Date.now() + timeoutMs;
	while (Date.now() < deadline) {
		if (await canConnect(PGLITE_PORT, PGLITE_HOST)) return;
		await new Promise((r) => setTimeout(r, 50));
	}
	throw new Error(`PGlite socket server did not become ready on ${PGLITE_HOST}:${PGLITE_PORT}`);
}

// 実PostgreSQLの代わりにWASM版PostgresであるPGliteをソケットサーバーとして立て、
// 通常のPostgresクライアント（Prisma含む）から専用ポートとして接続させる。
// スキーマ自体はschema.prismaのまま（postgresql provider）使えるため、
// アプリ側コードは実DBに対する接続と何ら変わらない。
export async function startPGliteServer(): Promise<void> {
	db = await PGlite.create();
	server = new PGLiteSocketServer({ db, port: PGLITE_PORT, host: PGLITE_HOST });
	await server.start();
	await waitUntilAcceptingConnections();

	// execFileSync(同期実行)はNode.jsのイベントループを止めてしまい、PGliteが
	// 同一プロセス内で処理する接続応答が進まなくなるため、非同期版を使う。
	try {
		await execFileAsync(
			"pnpm",
			["exec", "prisma", "db", "push", "--skip-generate", "--accept-data-loss"],
			{
				cwd: TABLES_ROOT,
				env: { ...process.env, DATABASE_URL: PGLITE_DATABASE_URL },
			},
		);
	} catch (error) {
		const { stdout, stderr } = error as { stdout?: string; stderr?: string };
		// eslint-disable-next-line no-console -- db push失敗時の原因はprismaの標準出力にしか出ないため
		console.error(stdout, stderr);
		throw error;
	}
}

export async function stopPGliteServer(): Promise<void> {
	await server?.stop();
	await db?.close();
	server = null;
	db = null;
}
