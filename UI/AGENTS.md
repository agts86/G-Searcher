# UI Agent Guide

このファイルは `UI/` 配下専用のルールを定義する。

## 1. 技術スタック前提

- Next.js（TypeScript、`strict` モード）
- レンダリング: `output: 'export'` による静的出力 → CSR（SPA）
- パッケージマネージャー: pnpm
- データ取得: React Query / TanStack Query（Query Key は feature 単位）
- 地図: react-leaflet（CSR 向け）
- Lint: ESLint v9 + `@typescript-eslint`
- Format: Prettier
- Test: Jest + ts-jest

## 2. 実装ルール

- 画面固有処理は `features/` に集約する
- API 通信は `lib/api/` に集約し、コンポーネントから直接 `fetch` を乱立させない
- 共通型は `types/` または各 feature 内に置き、重複定義を避ける
- 共通 UI は `components/ui/`、業務 UI は `components/logs/` など用途別に分ける
- 認証判定は UI 側で一元管理し、未認証時は `/login` へ遷移する

## 3. コーディング規約

- `any` 禁止（必要時は理由コメント必須）
- すべての公開関数で戻り値型を明示する
- 型のみ import は `import type` を使う
- ネストは3階層以下（早期 return で浅くする）
- SSR 前提機能（`getServerSideProps` 相当）は使用しない

## 4. チェックコマンド

- `pnpm lint`
- `pnpm build`
- `pnpm test`

## 5. 関連仕様

- 詳細仕様: `UI/docs/INSTRUCTION.md`
- コマンド辞書: `docs/agent/commands.md`
- タグ辞書: `docs/agent/tags.md`
