# LineWebHookAPI

## 概要

LineChatBotのフック先に使うAPI<br/>

https://lin.ee/sOGflWx

## 作成の背景

ナイツーや飲み会後の締めのラーメン屋を探すのに苦労したので作りました。<br/>
UIデザインが得意でないためUI作成の手間省きでLineChatBot,Line Messaging APIを使うことにしました<br/>

## 使用技術

■ 言語・FW<br>

-   C#：ASP.NET Core API
-   Bat
-   TypeScript：Nuxt.js or Next.js(予定。後で利用履歴管理画面を作ろうかなて思ってます）

■ DB

-   PostgreSQL

■ クラウド

-   WEB:Back4App（コスト次第でAws LambdaかAzure CloudFunctionsに移行する）
-   DB:Supabase

■ コンテナ

-   docker | docker-compose

■ その他<br>

-   HotPepper API
-   YOLP API
-   Line Messaging API

## 前提

1. HotPepperAPIのキーを取得済み
2. YOLP APIのキーを取得済み
3. Line公式アカウント、Line Developerアカウント開設済み（デバッグ時は不要）

## 実行方法(WindowsOSでWSL上で動かす想定)

1. .NETSDKのインストール(Windows端末で)

  https://dotnet.microsoft.com/ja-jp/download

2. 証明書の発行(Windows端末で)
  ```
      cd /API
      Dev-Certs-Link.bat
  ```
3. 設定ファイルの作成

`appsettings.json`をコピーし`appsettings.Development.json`にリネームして以下を修正
  ```
  {
    "ConnectionStrings": {
      "PostgreSQLConnection": "{docker-compose使わない場合は任意に設定)}"
    },
    "HotPepper": {
      "Url": "https://webservice.recruit.co.jp/hotpepper/{0}/v1/?key={1}&format=json",
      "Key": {取得したHotPepperのAPIキー)
    },
    "Line": {
      "Url": "https://api.line.me/v2/bot/message/{0}",
      "Token": {取得したLine Developersのチャネルアクセストークン),
      "ChannelSecret": {取得したLine 公式アカウントのチャンネルシークレット)
    },
    "Yahoo": {
      "Url": "https://map.yahooapis.jp/search/local/V1/{0}?appid={1}&output=json&detail=full",
      "Key": "{取得したYOLPのAPIキー)"
    }
  }
  ``` 

4. ビルド
   
  ```
      docker-compose build
  ```    
5. コンテナ起動（実行）
  ```
      docker-compose up -d
  ``` 
6. コンテナ停止
  ```
      docker-compose down
  ``` 

7. 確認

  https://localhost:5001/swagger/index.html

8. DBマイグレード(初回だけ)
  ```
      docker-compose exec backend bash
      cd src/
      dotnet ef database update
  ```


