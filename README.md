# LineWebHookAPI

## 概要

LineChatBotのフック先に使うAPI<br/>

https://lin.ee/sOGflWx

## 作成の背景

ナイツーや飲み会後の締めのラーメン屋を探すのに苦労したので作りました。

## 使用技術

■ 言語・FW<br>

-   C#：ASP.NET Core API
-   TypeScript：Next.js
-   Bat
-   PowerShell

■ DB

-   postgres

■ コンテナ

-   docker | docker-compose

■ その他<br>

-   YOLP API
-   Line Messaging API

## API構成

- `API/Host` : Host（`Program.cs` / Middleware / 起動設定）
- `API/Features` : Feature単位（Auth / Managed / Webhook）の Controller / Service / Dto
- `API/Tables` : DB テーブル
- `API/Shared` : 共通 Validation / Exception / Utility / Interface
- `API/Infrastructure` : Repository 実装 / DbContext / Migrations

## 前提

1. YOLP APIのキーを取得済み
2. Line公式アカウント、Line Developerアカウント開設済み（デバッグ時は不要）

## 実行方法

1. 証明書の発行
      
   ※WSLで利用する場合WSL側とホスト側の改行コードが違う場合がありますので実行環境側の改行コードに適宜変換してください
   1. Powershellが使える場合
      
      `API/Dev-Certs-Link.ps1`をホスト側にコピーして管理者権限で実行
      
      ```
      （必要に応じて）Unblock-File -Path . "{配置したPath}"
      （必要に応じて）Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process
      .\Dev-Certs-Link.ps1
      ```
   3. Windows端末でホスト側に.NETがインストールされている場合
      
      `API/Dev-Certs-Link.bat`をホスト側にコピーして管理者権限で実行
      
      ```
      .\Dev-Certs-Link.bat
      ```

3. 設定ファイルの作成

`appsettings.json`をコピーし`appsettings.Development.json`にリネームして以下を修正
  ```
  {
    "ConnectionStrings": {
      "PostgreSQLConnection": "{docker-compose使わない場合は任意に設定)}"
    },
    "Line": {
      "Token": {取得したLine Developersのチャネルアクセストークン),
      "ChannelSecret": {取得したLine 公式アカウントのチャンネルシークレット)
    },
    "Yahoo": {
      "AppId": "{取得したYOLPのAPIキー)"
    },
    "Auth": {
      "AdminUserName": "{管理画面の管理者ユーザー名}",
      "AdminPassword": "{管理画面の管理者パスワード}",
      "JwtKey": "{32文字以上の任意の秘密鍵}",
      "Issuer": "LineWebHookAPI",
      "Audience": "LineWebHookAdmin",
      "ExpiresMinutes": 15,
      "RefreshExpiresDays": 7
    }
  }
  ``` 

3. ビルド
   
  ```
      docker-compose build
  ```    
4. コンテナ起動（実行）
  ```
      docker-compose up -d
  ``` 
5. コンテナ停止
  ```
      docker-compose down
  ``` 

6. API確認

  https://localhost:5001/swagger/index.html

7. UI確認

  http://localhost:3000/login/

8. DBマイグレード(初回だけ)
  ```
      docker-compose exec backend bash
      cd /app/Host
      dotnet ef database update \
        --project ../Infrastructure/Infrastructure.csproj \
        --startup-project ./Host.csproj
  ```

## UI 開発（HTTPS 維持）

Next.js 開発サーバーは `UI/next.config.ts` の `rewrites` で `/api/:path*` を `API_BASE_URL`（未指定時は `https://localhost:5001`）へ中継する。  
このとき TLS 検証は Node.js 側で行われるため、開発時は `NODE_EXTRA_CA_CERTS` の設定が必要。

認証は `Access Token 15分 + Refresh Token 7日`。  
UI は有効期限の5分前に `/api/v1/auth/refresh` を呼び、失敗時は `401` で再ログインへ遷移する。

VSCode の `UI + API: 同時起動` を使う場合は、以下が自動で適用される。

- `prepare-ui-dev-ca` タスクで `${USERPROFILE}/.aspnet/https/WslLocalhost.pfx` から `/tmp/linewebhook-cert/localhost-dev-root-ca.pem` を生成
- `UI: Next.js dev` 起動時に `NODE_EXTRA_CA_CERTS=/tmp/linewebhook-cert/localhost-dev-root-ca.pem` を設定

VSCode を使わずに `UI` を起動する場合は、先に PEM を生成してから起動する。

```bash
mkdir -p /tmp/linewebhook-cert
openssl pkcs12 -in "${USERPROFILE}/.aspnet/https/WslLocalhost.pfx" \
  -nokeys -cacerts -passin pass:WslLocalhost \
  -out /tmp/linewebhook-cert/localhost-dev-root-ca.pem

cd UI
NODE_EXTRA_CA_CERTS=/tmp/linewebhook-cert/localhost-dev-root-ca.pem pnpm dev -- --port 3000
```

## デプロイ用コンテナ

`Dockerfile` は multi-stage build で `UI` をビルドし、生成物（`UI/out`）を API コンテナの `wwwroot` に同梱する。  
そのため、コンテナビルド時の context はリポジトリルート（`.`）を使用する。
