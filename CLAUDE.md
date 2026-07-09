# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

このファイルはリポジトリ全体の共通ルールを定義する。
`api/` 配下の作業では [api/CLAUDE.md](api/CLAUDE.md)、`ui/` 配下では [ui/CLAUDE.md](ui/CLAUDE.md) も参照すること。

## プロジェクト概要

LINE Messaging API の Webhook を受ける Hono API と、管理画面の Next.js SPA のモノレポ。
LINE ユーザーが位置情報やメッセージを送信すると、Yahoo!ローカルサーチ API で周辺のグルメ情報を検索して返信する。
API はもともと C#/ASP.NET Core で実装されていたが、TypeScript(Hono + Prisma) へ完全移行済み（旧 `API/` は削除済み）。

### 技術スタック

- **API**: TypeScript / Hono / Prisma / PostgreSQL（pnpm workspace モノレポ、`api/`）
- **UI**: TypeScript / Next.js (`output: 'export'` で静的出力 → CSR SPA)
- **デプロイ**: Docker multi-stage build → GHCR → Azure Web App（API + UI を単一コンテナで配信）
- **外部連携**: LINE Messaging API, Yahoo!ローカルサーチ API (YOLP)
- **cron**: Cloudflare Workers（`cron-worker/CloudFlare`、ヘルスチェック）

## アーキテクチャ

### API レイヤ構成（pnpm workspace）

依存方向: `Controller(routes) → Service → Repository`（一方向のみ）

| パッケージ | 役割 | 参照先 |
|---|---|---|
| `api/src/host` | Hono app組み立て / DI配線 / 起動設定 | Features, Infrastructure |
| `api/src/features/*` | Feature単位の Router / Service / DTO（auth, managed, webhook） | Tables, Shared, 同一Feature内 |
| `api/src/tables` | Prisma スキーマ | Shared |
| `api/src/shared` | 共通 Validation / Utility / Interface | なし（最内層） |
| `api/src/infrastructure` | Repository 実装 / Prisma Client / 外部APIクライアント | Tables, Features, Shared |

pnpmの非hoisted node_modulesとeslint-plugin-boundariesで、この参照方向を物理的・lintレベルの両方で強制している。

### UI ディレクトリ構成

- `src/app/` — ページ（`(auth)/login`, `(dashboard)/dashboard`）
- `src/features/` — 画面固有ロジック（auth, gourmet-location, gourmet-word, error-log, job-log）
- `src/lib/api/` — API クライアント集約
- `src/components/` — 共通UI (`ui/`), 業務UI (`logs/`, `map/`)

### 認証フロー

Access Token 15分 + Refresh Token 7日（DB 管理）。UI は有効期限5分前に `/api/v1/auth/refresh` を呼び、失敗時は再ログインへ遷移。

## コマンド

### API（api）

```bash
cd api
pnpm -r build   # 全パッケージビルド（型チェック含む）
pnpm -r lint    # ESLint（全パッケージ）
pnpm -r test    # テスト全件（vitest）
pnpm --filter @api/<package-name> test  # パッケージ個別（例: @api/features-webhook）

# DBスキーマ同期（ローカル/CI用。migration履歴は持たずschema.prismaへ同期するだけ）
pnpm --filter @api/tables exec prisma db push
```

### UI

```bash
cd ui
pnpm lint       # ESLint
pnpm build      # 静的出力（out/）
pnpm test       # Jest
```

### Docker（ローカル開発）

```bash
docker compose build
docker compose up -d    # postgres:5432, api:3001, ui:3000
docker compose down
```

## 適用範囲と優先順位

- ルート `CLAUDE.md` は全体ルール
- `api/CLAUDE.md` / `ui/CLAUDE.md` は各サブディレクトリでこのファイルより優先
- 競合時は「より深い階層」のルールを優先

## 共通ルール

- 変更は最小差分で行う
- 関連のないファイルは触らない
- 生成物は原則コミットしない（`dist/`, `node_modules/`, `.next/` など）
- パス指定は必ず `/` を使い、`\` は使わない（Linux/WSL で異常パスを防ぐ）
- 高リスク操作（大量削除、履歴改変）は明示合意がある場合のみ行う
- 仕様変更時はドキュメントも同時更新する

## コミュニケーションの原則

- すべての回答・説明・提案は日本語で行う
- 技術用語は必要に応じて英語表記を併記してよいが、説明本文は日本語を優先する
- 曖昧な断定を避け、根拠と前提を明示する
- 実行できなかった作業は「未実施理由」と「次の実行手順」を必ず示す

## 運用ドキュメント

@.claude/rules/tags.md

## 依頼テンプレート

```
#api #db #migration 目的: xxx 制約: yyy
```
