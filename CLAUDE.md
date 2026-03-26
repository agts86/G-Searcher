# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

このファイルはリポジトリ全体の共通ルールを定義する。
`API/` 配下の作業では [API/CLAUDE.md](API/CLAUDE.md)、`UI/` 配下では [UI/CLAUDE.md](UI/CLAUDE.md) も参照すること。

## プロジェクト概要

LINE Messaging API の Webhook を受ける ASP.NET Core API と、管理画面の Next.js SPA のモノレポ。
LINE ユーザーが位置情報やメッセージを送信すると、Yahoo!ローカルサーチ API で周辺のグルメ情報を検索して返信する。

### 技術スタック

- **API**: C# / ASP.NET Core (.NET 10) / Entity Framework Core / PostgreSQL
- **UI**: TypeScript / Next.js (`output: 'export'` で静的出力 → CSR SPA)
- **デプロイ**: Docker multi-stage build → GHCR → Azure Web App（API + UI を単一コンテナで配信）
- **外部連携**: LINE Messaging API, Yahoo!ローカルサーチ API (YOLP)
- **cron**: Cloudflare Workers（`cron-worker/CloudFlare`、5分間隔でヘルスチェック）

## アーキテクチャ

### API レイヤ構成（5プロジェクト + Test）

依存方向: `Controller → Service → Repository`（一方向のみ）

| プロジェクト | 役割 | 参照先 |
|---|---|---|
| `API/Host` | Program.cs / DI 登録 / Middleware / 起動設定 | Features, Infrastructure |
| `API/Features` | Feature単位の Controller / Service / DTO | Tables, Shared |
| `API/Tables` | DB テーブル | Shared |
| `API/Shared` | 共通 Validation / Exception / Utility / Interface | なし（最内層） |
| `API/Infrastructure` | Repository 実装 / DbContext / Migrations | Tables, Features, Shared |
| `API/Test` | xUnit テスト | 全プロジェクト |

### UI ディレクトリ構成

- `src/app/` — ページ（`(auth)/login`, `(dashboard)/dashboard`）
- `src/features/` — 画面固有ロジック（auth, gourmet-location, gourmet-word, error-log, job-log）
- `src/lib/api/` — API クライアント集約
- `src/components/` — 共通UI (`ui/`), 業務UI (`logs/`, `map/`)

### 認証フロー

Access Token 15分 + Refresh Token 7日（DB 管理）。UI は有効期限5分前に `/api/v1/auth/refresh` を呼び、失敗時は再ログインへ遷移。

## コマンド

### API

```bash
dotnet build API/Host/Host.csproj          # ビルド
dotnet test API/Test/Test.csproj           # テスト全件
dotnet test API/Test/Test.csproj --filter "FullyQualifiedName~ClassName"  # テスト個別

# Migration
dotnet tool run dotnet-ef migrations add <Name> \
  --project API/Infrastructure/Infrastructure.csproj \
  --startup-project API/Host/Host.csproj \
  --output-dir Migrations

dotnet tool run dotnet-ef migrations remove \
  --project API/Infrastructure/Infrastructure.csproj \
  --startup-project API/Host/Host.csproj

dotnet tool run dotnet-ef database update \
  --project API/Infrastructure/Infrastructure.csproj \
  --startup-project API/Host/Host.csproj
```

### UI

```bash
cd UI
pnpm lint       # ESLint
pnpm build      # 静的出力（out/）
pnpm test       # Jest
```

### Docker（ローカル開発）

```bash
docker compose build
docker compose up -d    # postgres:5432, backend:5001, ui:3000
docker compose down
```

## 適用範囲と優先順位

- ルート `CLAUDE.md` は全体ルール
- `API/CLAUDE.md` / `UI/CLAUDE.md` は各サブディレクトリでこのファイルより優先
- 競合時は「より深い階層」のルールを優先

## 共通ルール

- 変更は最小差分で行う
- 関連のないファイルは触らない
- 生成物は原則コミットしない（`bin/`, `obj/` など）
- パス指定は必ず `/` を使い、`\` は使わない（Linux/WSL で `bin\Debug` のような異常パスを防ぐ）
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
