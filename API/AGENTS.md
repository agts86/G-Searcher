# API Agent Guide

このファイルは `API/` 配下専用のルールを定義する。

## 1. 技術スタック前提

- ASP.NET Core Web API (`net10.0`)
- EF Core + PostgreSQL (`UseNpgsql`)
- Migration は `dotnet-ef` を使用

## 2. 実装ルール

- 依存方向は `Controller -> Service -> Repository` を維持
- `Host/Program.cs` に DI 設定を集約
- Feature / Infrastructure の DI 登録は公開拡張メソッド（例: `AddAuthFeature`, `AddAuthInfrastructure`）経由を優先し、`Host` から実装型を直接参照しない
- プロジェクト参照は `Features -> (Tables, Shared)`, `Infrastructure -> (Tables, Features, Shared)`, `Host -> (Infrastructure, Features)` を維持
- `Features` / `Tables` / `Shared` から `DbContext` を直接参照しない
- `Features` では Controller / API 入出力 DTO を `public`、それ以外の実装（Service / Repository / helper）は `internal` を基本とする
- DB アクセスは `LineWebHookContext` 経由で統一
- 設定キーは `ConnectionStrings:PostgreSQLConnection` を使用
- 環境別設定は `appsettings.Development.json` を使用

## 3. Migration ルール

- Migration ファイルの手編集は禁止
- 変更は `dotnet tool run dotnet-ef migrations add/remove` で実施
- 変更後は `LineWebHookContextModelSnapshot` の整合を確認

## 4. チェックコマンド

- `dotnet build API/Host/Host.csproj`
- `dotnet test API/Test/Test.csproj`

## 5. 関連仕様

- 詳細仕様: `API/docs/INSTRUCTION.md`
- コマンド辞書: `docs/agent/commands.md`
- タグ辞書: `docs/agent/tags.md`
