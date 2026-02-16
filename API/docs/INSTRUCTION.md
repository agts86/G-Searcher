# LineWebHookAPI 設計原則・コーディング規約指示書

## 1. 目的

このドキュメントは `API/` 配下の API 実装ルールを定義する。  
対象は ASP.NET Core Web API（.NET 10 / C#）であり、TypeScript / Next.js / NestJS 前提の規約は適用しない。

## 1.1 MUSTルール（最優先）

- すべての作業完了後に、`dotnet build` と `dotnet test` を必ず実行する
- 実行できなかった場合は、理由（環境制約・依存不足など）を必ず記録する

## 2. レイヤ構成と依存方向

### ✅ やるべきこと

- プロジェクトを最小 4 構成で分割する  
  `Host`（Program / 起動設定）  
  `Presentation`（Controller / Middleware / Validation）  
  `Application`（Service / DTO / Exception / Repository Interface）  
  `Infrastructure`（Repository 実装 / DbContext / Migration）
- `Presentation` は HTTP 入出力と認証・バリデーションに専念し、ビジネスロジックを持たない
- `Application` はユースケースの実装を担当し、データ取得はリポジトリインターフェース経由で行う
- `Infrastructure` は DB へのアクセスのみを担当し、`LineWebHookContext` 経由で操作する
- 依存は `Controller -> Service -> Repository` の一方向を維持する
- 参照方向は `Presentation -> Application`, `Infrastructure -> Application`, `Host -> (Presentation, Application, Infrastructure)` を維持する

### ❌ やってはいけないこと

- Controller から `DbContext` を直接参照する
- Service 内で SQL 文字列や EF クエリを直接持つ（Repository に集約する）
- Repository で HTTP 呼び出しや業務ルール判定を行う
- レイヤをまたいだ循環参照を作る

## 3. DI（依存性注入）ルール

### ✅ やるべきこと

- DI 登録は `Host/Program.cs` に集約する
- インターフェース経由で依存を受ける（例: `IYahooService`, `IManagedRepository`）
- ライフタイムは用途で使い分ける
- HTTP クライアントは `AddHttpClient` で登録する

### ❌ やってはいけないこと

- `new` で依存クラスを直接生成する
- static に状態を保持して疑似 DI として使う
- 同一責務のサービスを複数箇所で重複登録する

## 4. DB・Migration 運用

### ✅ やるべきこと

- DB プロバイダは PostgreSQL（`UseNpgsql`）を前提とする
- 接続文字列キーは `ConnectionStrings:PostgreSQLConnection` を使用する
- Migration は `dotnet ef` コマンドで生成し、手編集しない
- 型・スキーマ変更時は `Migrations/` と `LineWebHookContextModelSnapshot` を必ず整合させる

### ❌ やってはいけないこと

- SQLite 前提の型（`TEXT`, `REAL` など）を PostgreSQL マイグレーションに混在させる
- Migration の一部だけを手で修正してスナップショットと不整合にする
- 環境ごとに接続文字列キー名を変える

## 5. 設定ファイル運用

### ✅ やるべきこと

- 共通設定は `appsettings.json` に置く
- 開発用上書きは `appsettings.Development.json` を使う
- 秘密情報はコミットせず、環境変数や Secret 管理を利用する
- `ASPNETCORE_ENVIRONMENT=Development` とファイル名の大文字小文字を一致させる

### ❌ やってはいけないこと

- 実運用のトークンやパスワードを設定ファイルへ直書きする
- `appsettings.development.json` のような誤記ファイルを増やす
- 環境別設定をコード内でハードコードする

## 6. コードスタイル

### ✅ やるべきこと

- Public メソッドには戻り値型を明示し、非同期は `Task<T>` / `Task` を使う
- Nullable 参照型の警告を無視せず、`?` と null チェックを適切に使う
- 例外メッセージは原因が追える内容にする
- ログは再現・調査に必要な文脈（対象ID、処理名、例外）を残す

### ❌ やってはいけないこと

- `catch (Exception)` で握りつぶす
- 調査不能な曖昧メッセージだけを返す
- マジック文字列やマジックナンバーを散在させる

## 7. Controller 実装ルール

### ✅ やるべきこと

- リクエスト検証は `Validations/` や属性で行う
- `ActionResult` で HTTP ステータスを明示する
- 外部連携エラーは適切なステータスへ変換する（4xx/5xx）
- `/health` など運用用エンドポイントは軽量に保つ
- Controller の public メソッドは HTTP エンドポイントに限定する
- Cookie 設定生成・レスポンス整形などの補助ロジックは `Configurations/` や `Utilities/` に分離する
- 認証失敗や業務エラーは Service 層で `StatusCodeException` 派生例外を送出し、`Middleware` で HTTP レスポンスへ変換する

### ❌ やってはいけないこと

- Controller で長い分岐・複雑ロジックを実装する
- 例外をそのまま外に漏らす
- 仕様化されていないレスポンス形を都度追加する
- Controller 内に private helper メソッドを増やして関心事を混在させる
- Controller で `Unauthorized()` / `BadRequest()` などのエラー応答を直接返す

## 8. 設計パターンと複雑度管理の自動適用

### ✅ 自動適用すべきルール

- ストラテジー解決は `API/Application/Utilities/Polymorphism.cs` の `Polymorphism.CreatePolymorphismArray<T>()` を優先して使用する
- 基底クラスまたはインターフェースを指定して実装群を収集し、条件に一致する実装を選択する
- `switch` や長い `if-else` で処理種別を分岐している場合、Strategy パターンやポリモーフィズムへの置換を優先する
- 条件に応じた生成が増える場合は Factory パターンを検討する
- 名前や条件で処理を選ぶ場合は `IEnumerable<T>` + `FirstOrDefault` / `SingleOrDefault` パターンで分岐集中を避ける
- ネストが深い場合はガード節（早期 return）で浅くする
- メソッドの責務が複数にまたがる場合は（Controller を除き）private メソッドやサービス分割で責務を分離する

### 🤖 自動判断基準

- `switch (type)` や `if (name == "...")` が3件以上ある場合、パターン置換を提案する
- `if` のネストが3階層以上になった場合、ガード節やメソッド分割を提案する
- 1メソッドが50行を超える場合、責務分割を提案する
- 循環的複雑度が10を超えそうな場合、実装前に分割方針を確定する

### ✅ ASP.NET Core / C# での適用例

```csharp
using LineWebHookAPI.Utilities;

public interface IYahooTool
{
    string Name { get; }
    Task<ToolResult> ExecuteAsync(ToolRequest request, CancellationToken cancellationToken);
}

public class YahooToolService(IYahooRepository repository)
{
    private IYahooTool[] Tools { get; } = Polymorphism.CreatePolymorphismArray<IYahooTool>(repository);

    public async Task<ToolResult> ExecuteAsync(string toolName, ToolRequest request, CancellationToken cancellationToken)
    {
        var tool = Tools.FirstOrDefault(x => x.Name == toolName);
        if (tool is null)
        {
            throw new ArgumentException($"Unknown tool: {toolName}");
        }

        return await tool.ExecuteAsync(request, cancellationToken);
    }
}
```

### ❌ やってはいけないこと

- 分岐追加のたびに `switch` / `if-else` を肥大化させる
- 複雑度10超のメソッドを相談なく追加する
- 3階層を超えるネストを放置する
- `Polymorphism.CreatePolymorphismArray<T>()` で代替できる分岐を手書きで増やし続ける

## 9. テストコードのルール

### ✅ やるべきこと

- テストは1ケース1意図で、分岐のない直線的な記述を基本とする
- テストデータ作成ヘルパー（メソッド外定義）は「データ生成のみ」に限定する
- 永続化が必要な場合、`SaveChanges` / `SaveChangesAsync` は各テストメソッド内で実行する
- テストデータ作成ヘルパーは `Action<T>` で上書き可能にし、共通初期値 + 必要部分だけ変更する

```csharp
private static GourmetWordLog CreateGourmetWordLog(Action<GourmetWordLog> setup = null)
{
    var entity = new GourmetWordLog
    {
        Text = "default"
    };

    setup?.Invoke(entity);
    return entity;
}
```

### ❌ やってはいけないこと

- テストメソッド内で `if` 文を使う
- テストメソッド内で `switch` 文を使う
- テストデータをメソッド外へ切り出した際に、そのメソッド内で `SaveChanges` / `SaveChangesAsync` を実行する

## 10. 実装変更時のチェックリスト

- レイヤ依存方向（Controller -> Service -> Repository）が守られている
- DI 登録（`Program.cs`）の追加漏れがない
- DB 変更時に Migration が `dotnet ef` で再生成されている
- 設定キー名（`PostgreSQLConnection`）が統一されている
- `dotnet build` を実行して成功している
- `dotnet test` を実行して成功している

## 11. 推奨コマンド

```bash
# API プロジェクトのビルド
dotnet build API/Host/Host.csproj
dotnet test API/Test/Test.csproj

# Migration 追加（リポジトリルートで実行）
cd /path/to/repo
dotnet tool run dotnet-ef migrations add <MigrationName> \
  --project API/Infrastructure/Infrastructure.csproj \
  --startup-project API/Host/Host.csproj \
  --output-dir Migrations

# Migration 適用
cd /path/to/repo
dotnet tool run dotnet-ef database update \
  --project API/Infrastructure/Infrastructure.csproj \
  --startup-project API/Host/Host.csproj
```
