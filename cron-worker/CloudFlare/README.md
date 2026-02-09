# cron-worker / CloudFlare

Cloudflare Workers の `scheduled` トリガーで、API のヘルスチェックエンドポイントを定期実行するための Worker。

## 仕様

- 実装: `src/index.ts`
- 実行間隔: 5分 (`*/5 * * * *`)
- 送信先: `TARGET_URL`（Secret / local env）
- リクエスト: `GET` + `User-Agent: cf-cron-warmup`
- 挙動: 非 2xx の場合はログ出力

## 前提

- Node.js / npm が使えること
- Cloudflare へ `wrangler` でログイン済みであること

## セットアップ

```bash
cd cron-worker/CloudFlare
npm install
```

## ローカル実行（scheduled の手動トリガー）

1. ローカル環境変数ファイルを作成

```bash
cp .dev.vars.example .dev.vars
```

2. `TARGET_URL` を必要に応じて変更

```dotenv
TARGET_URL=https://line-webhook-api.azurewebsites.net/health
```

3. Worker をローカル起動

```bash
npm run dev:scheduled
```

4. 別ターミナルから scheduled を手動実行

```bash
npm run trigger:scheduled
```

## テスト / 型チェック

```bash
npm run test -- --run
npx tsc --noEmit
```

## Secret 設定（本番/共有環境）

`TARGET_URL` は平文 `vars` ではなく Secret で管理する。

```bash
printf '%s' '[TARGET_URL]' | npx wrangler secret put TARGET_URL --name cron-worker
npx wrangler secret list --name cron-worker
```

環境を分けている場合は `--env <env-name>` を付与する。

## デプロイ

```bash
npm run deploy
```

## 補足

- Dashboard 側に同名の Variable (`TARGET_URL`) が残っていると、`secret put` が失敗する。
- その場合は Variable を削除してから Secret を再作成する。
