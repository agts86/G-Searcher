# api-ts 設計原則・コーディング規約指示書

## 1. 目的

このドキュメントは `api-ts/` 配下の実装ルールを定義する。
対象は Hono（TypeScript）で実装するAPIで、`API/`（.NET）からのストラングラーフィグ移行の一部。
`API/`の5プロジェクト分割（Host/Features/Tables/Shared/Infrastructure）と同じ考え方を、pnpm workspaceの複数パッケージで再現する。

**重要**: レイヤ・Feature間の分離は、書面のルールだけに頼らない。`API/`の5プロジェクト分割が「Controller/ServiceからDbContextを直接使わせない」ために物理的な壁（コンパイラのプロジェクト参照）として機能しているのと同じ理由で、`api-ts/`でも**各パッケージの`package.json`の`dependencies`を物理的な壁として使う**。pnpmはデフォルトで`node_modules`をhoistしないため、パッケージが宣言していない依存はimportしようとした時点で解決できずビルド/実行が失敗する（lintより強い、コンパイルエラー相当の強制力）。

## 1.2 Node組み込み型（`@types/node`）

- `node:crypto`や`process`など Node.js 組み込みAPIを使うパッケージは、`@types/node`を**自分自身の`package.json`のdevDependenciesに**宣言する（ルートにあるだけでは各パッケージから見えない、pnpmの厳格な依存分離のため）
- `tsconfig.base.json`に`"types": ["node"]`を明示している。`composite: true`のプロジェクト参照構成では、`@types`配下の自動検出が働かない場合があるため、自動検出に頼らず明示する

## 1.1 MUSTルール（最優先）

- すべての作業完了後に `pnpm test`（workspace全体）を必ず実行する
- 実装前にテストを書く（TDD）。テストが無い状態で実装コードを追加しない
- 新しいパッケージ間の依存を追加する前に、本ドキュメントの依存方向ルール（4章）に違反しないか確認する
- 実行できなかった場合は、理由を必ず記録する

## 2. ディレクトリ構成

```
api-ts/
  src/
    host/                  # ~ API/Host: 起動・DI配線（Program.cs相当）
      src/
        app.ts                 # Honoアプリ組み立て、featureのルーターをmount
        main.ts                  # サーバー起動エントリポイント
    features/
      auth/                  # ~ API/Features/Auth
        src/
          auth.routes.ts          # Honoルーター（HTTP入出力のみ）
          auth.service.ts           # ビジネスロジック
          auth.repository.ts          # インターフェースのみ（実装を持たない）
          auth.dto.ts                   # Zodスキーマ
        test/
      managed/                # ~ API/Features/Managed（実装済み）。auth/と同じ形
      webhook/                  # 未着手。実装時はauth/と同じ形にする
    tables/                 # ~ API/Tables: Prismaスキーマ・生成される型
      prisma/schema.prisma
    shared/                  # ~ API/Shared: 共通utility（jwt.ts, cookies.ts, hash.ts）
      src/
      test/
    infrastructure/           # ~ API/Infrastructure: featuresのrepositoryインターフェースをPrismaで実装
      src/
  pnpm-workspace.yaml
  package.json              # ルート、workspace全体のスクリプト
```

## 3. パッケージ命名

- `@api-ts/host`, `@api-ts/features-auth`, `@api-ts/tables`, `@api-ts/shared`, `@api-ts/infrastructure` のように`@api-ts/`スコープで統一する

## 4. 依存方向ルール（`API/`のプロジェクト参照ルールと1:1対応、`package.json`の`dependencies`で強制する）

| パッケージ | 依存してよいもの | 依存してはいけないもの |
|---|---|---|
| `features/*` | `tables`（型のみ）, `shared` | `infrastructure`, `@prisma/client`, 他の`features/*` |
| `infrastructure` | `tables`, `shared`, `features/*`（interfaceの型を知るため） | — |
| `host` | `features/*`, `infrastructure`, `shared` | — |
| `tables` | （なし、最内層） | — |
| `shared` | （なし、最内層） | — |

- `features/auth`が`infrastructure`や`@prisma/client`をimportしたくなった場合、それは設計違反のサイン。`auth.repository.ts`にインターフェースを追加し、`infrastructure`側で実装する
- `host`が実際のRepository実装インスタンスを生成し、Serviceのコンストラクタ引数として渡す（DIコンテナが無いため、ここが唯一の「配線」箇所になる）
- Feature間（例: `features/auth`が`features/managed`を参照する）は禁止。共有したいロジックがあれば`shared`に上げる

## 5. バージョニング

- `app.route('/api/v1/<resource>', v1Router)`のようにバージョン別ルーターを`host/src/app.ts`で明示的にマウントする
- 新バージョンが必要になったら、既存ルーターをコピーせず新規ファイルを作り並行稼働させる

## 6. OpenAPI / Swagger

- `@hono/zod-openapi`の`OpenAPIHono`を使う。リクエスト/レスポンスは各featureの`*.dto.ts`のZodスキーマで定義し、バリデーションとOpenAPI仕様生成の両方に使い回す
- `/doc`でOpenAPI JSON、`/ui`で`@hono/swagger-ui`を公開する

## 7. 認可（JWT検証）

- 認証必須エンドポイントは、`shared`の共通Honoミドルウェアで保護する。エンドポイントごとに検証コードを書き直さない
- ミドルウェアは`.NET`側`[Authorize]`と同じ検証項目（署名・issuer・audience・有効期限・clockSkew）を満たすこと
- JWT発行・検証ロジックは`shared/src/jwt.ts`に集約する

## 8. エラーハンドリング

- 認可・業務エラーは`HTTPException`（Hono組み込み）を投げる
- エラーメッセージは原因が追える内容にする（既存.NET側`Shared/Exceptions`の方針を踏襲）

## 9. コーディングスタイル

- Publicな関数は戻り値の型を明示する
- `any`は使わない。型が絞れない場合はZodスキーマから`z.infer`で導出する
- 1関数1責務を維持する。50行を超えたら分割を検討する

## 10. テスト

- フレームワーク: Vitest（workspace root から `pnpm -r test` で全パッケージ横断実行）
- ユニットテスト: `shared/`配下の純粋関数（JWT生成/検証、Cookie属性生成、ハッシュ化）はDBなしでテストする
- 統合テスト: `features/*/test/*.routes.test.ts`はHonoの`app.request()`で実際のルーティングを通してテストする。ローカルの`docker-compose.yml`のPostgresを使う（モックしない、`API/Test`がSQLite in-memoryの実DBを使う方針と同じ考え方）
- 既存`API/Test/`にある期待仕様と振る舞いが一致することを、移行対象ごとに確認する

## 11. 環境変数

- 秘密情報（`JWT_SECRET`, `ADMIN_PASSWORD`, `DATABASE_URL`等）はコードにハードコードしない。`.env.example`に必要なキー名だけを記載し、値は空にする
- 既存.NET側の設定キー（`Auth:JwtKey`等）と対応する環境変数名は、実装コードのコメントで対応関係を明示する

## 12. コミュニケーション規約

- 説明・提案・レビューは日本語で行う
- 未実施の作業は「未実施理由」と「次の実行手順」を必ず記載する
