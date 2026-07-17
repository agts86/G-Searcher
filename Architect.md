# アーキテクチャ図

## 1. システム全体（Runtime）

```mermaid
graph TD
    User["ユーザー（LINE）"]
    Admin["管理者ユーザー（Browser）"]
    LineAPI["LINE Messaging API"]
    Ui["UI（Next.js export）<br/>wwwroot で静的配信"]
    Api["server<br/>Hono (Node.js) / Prisma"]
    Yahoo["Yahoo!ローカルサーチ API (YOLP)"]
    DB["PostgreSQL"]

    User -->|位置情報/メッセージ| LineAPI
    LineAPI -->|Webhook| Api
    Admin -->|画面アクセス| Ui
    Ui -->|/api/v1/auth, /api/v1/managed| Api
    Api -->|ローカル検索| Yahoo
    Yahoo -->|検索結果| Api
    Api -->|ログ保存/参照| DB
    Api -->|Reply API| LineAPI
    LineAPI -->|返信| User
```

## 2. API 内部（Dependency、`server/`）

```mermaid
graph LR
    subgraph Host["src/host"]
      App["createApp() (app.ts)<br/>Hono配線・DI組み立て"]
    end

    subgraph Features["src/features/*"]
      WebhookRoutes["webhook.routes.ts"]
      WebhookService["WebhookService"]
      LineReplyService["LineReplyService"]
      AuthRoutes["auth.routes.ts"]
      AuthService["AuthService"]
      ManagedRoutes["managed.routes.ts"]
      ManagedService["ManagedService"]
    end

    subgraph Tables["src/tables"]
      Prisma["Prisma schema"]
    end

    subgraph Shared["src/shared"]
      SharedComp["JWT / Cookie / JSTタイムスタンプ / AuthGuard middleware"]
    end

    subgraph Infrastructure["src/infrastructure"]
      WebhookRepo["PrismaWebhookRepository"]
      AuthRepo["PrismaAuthRepository"]
      ManagedRepo["PrismaManagedRepository"]
      YolpClient["YolpClientImpl (fetch)"]
      LineReplyClient["LineReplyClientImpl"]
      HttpAdapter["HttpAdapter"]
      PrismaClient["PrismaClient"]
    end

    LineBotSdk["@line/bot-sdk LineBotClient"]
    Pg["PostgreSQL"]

    App --> WebhookRoutes --> WebhookService --> WebhookRepo
    WebhookService --> LineReplyService --> YolpClient
    LineReplyService --> LineReplyClient --> LineBotSdk
    App --> AuthRoutes --> AuthService --> AuthRepo
    App --> ManagedRoutes --> ManagedService --> ManagedRepo

    YolpClient --> HttpAdapter
    WebhookRepo --> PrismaClient
    AuthRepo --> PrismaClient
    ManagedRepo --> PrismaClient
    PrismaClient --> Pg

    WebhookService -. uses .-> Prisma
    AuthService -. uses .-> Prisma
    ManagedService -. uses .-> Prisma
    WebhookService -. uses .-> SharedComp
    AuthService -. uses .-> SharedComp
    ManagedService -. uses .-> SharedComp
```

pnpmの非hoisted node_modules + eslint-plugin-boundariesで、上記の参照方向（Host→Features→Tables/Shared、Infrastructure→Tables/Features/Shared）を物理的・lintレベルの両方で強制している。

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

    Api["server"]

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
- `Dockerfile` は multi-stage build で `web` と `server` をビルドし、`web/out` を server コンテナの `wwwroot` に、`server` 一式（node_modules込み）を同梱します。実行は `tsx` で `src/host/src/main.ts` を直接起動します。
- 認証は `Access Token + Refresh Token` を利用し、`RefreshToken` は DB で管理します。
