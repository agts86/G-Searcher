# アーキテクチャ図

## 1. システム全体（Runtime）

```mermaid
graph TD
    User["ユーザー（LINE）"]
    Admin["管理者ユーザー（Browser）"]
    LineAPI["LINE Messaging API"]
    Ui["UI（Next.js export）<br/>wwwroot で静的配信"]
    Api["LineWebHookAPI<br/>ASP.NET Core (.NET 10)"]
    Yahoo["Yahoo!ローカルサーチ API (YOLP)"]
    DB["PostgreSQL"]
    Worker["Cloudflare Workers<br/>cron-worker/CloudFlare (*/5分)"]

    User -->|位置情報/メッセージ| LineAPI
    LineAPI -->|Webhook| Api
    Admin -->|画面アクセス| Ui
    Ui -->|/api/v1/auth, /api/v1/managed| Api
    Api -->|ローカル検索| Yahoo
    Yahoo -->|検索結果| Api
    Api -->|ログ保存/参照| DB
    Api -->|Reply API| LineAPI
    LineAPI -->|返信| User
    Worker -->|GET /health| Api
```

## 2. API 内部（Dependency）

```mermaid
graph LR
    subgraph Presentation
      YahooController["YahooController"]
      AuthController["AuthController"]
      ManagedController["ManagedController"]
      Middleware["Middleware (IMiddleware)"]
    end

    subgraph Application
      YahooService["YahooService"]
      AuthService["AuthService"]
      ManagedService["ManagedService"]
      Queue["BackgroundJobQueue&lt;LocalJobDto&gt;"]
    end

    subgraph Infrastructure
      YahooRepo["YahooRepository"]
      AuthRepo["AuthRepository"]
      ManagedRepo["ManagedRepository"]
      MiddlewareRepo["MiddleWareRepository"]
      Bg["YahooBackgroundService"]
      DbContext["LineWebHookContext (DbContext)"]
    end

    LineSdk["LineMessagingClient"]
    YOLP["IYOLPClient"]
    Pg["PostgreSQL"]

    YahooController --> YahooService --> YahooRepo
    AuthController --> AuthService
    AuthService --> AuthRepo
    ManagedController --> ManagedService --> ManagedRepo
    Middleware --> MiddlewareRepo

    YahooController --> Queue
    Bg --> YahooService
    Queue --> Bg

    YahooRepo --> LineSdk
    YahooRepo --> YOLP
    YahooRepo --> DbContext
    AuthRepo --> DbContext
    ManagedRepo --> DbContext
    MiddlewareRepo --> DbContext
    DbContext --> Pg
```

## 3. UI 内部（Dependency）

```mermaid
graph LR
    subgraph Pages
      Login["(auth)/login"]
      Dashboard["(dashboard)/dashboard/*"]
    end

    subgraph Features
      UseAuth["useAuth / useAutoRefresh"]
      UseLogs["useGourmetLocation / useGourmetWord / useErrorLog / useJobLog"]
    end

    subgraph ApiClients
      AuthClient["auth-client"]
      ManagedClient["managed-client"]
      Client["api/client (401時はauth refresh再試行)"]
    end

    Api["LineWebHookAPI"]

    Login --> UseAuth
    Dashboard --> UseAuth
    Dashboard --> UseLogs
    UseAuth --> AuthClient --> Client
    UseLogs --> ManagedClient --> Client
    Client --> Api
```

## 4. デプロイ経路（CI/CD）

```mermaid
graph LR
    GH["GitHub Actions<br/>ContainerDeploy"]
    GHCR["GHCR"]
    Azure["Azure Web App<br/>line-webhook-api"]

    GH -->|Docker build/push（context: .）| GHCR
    GHCR -->|container deploy| Azure
```

注記:
- `Dockerfile` は multi-stage build で `UI` をビルドし、`UI/out` を API コンテナの `wwwroot` に同梱します。
- 認証は `Access Token + Refresh Token` を利用し、`RefreshToken` は DB で管理します。
