# LineWebHookAPI

## 概要

LineChatBotのフック先に使うAPI<br/>

https://lin.ee/sOGflWx

## 作成の背景

ナイツーや飲み会後の締めのラーメン屋を探すのに苦労したので作りました。<br/>
UIデザインが得意でないためUI作成の手間省きでLineChatBot,Line Messaging APIを使うことにしました<br/>

## 使用技術

■ 言語・FW<br>

-   C#(ASP.NET Core API)
-   Bat
-   TypeScript(Nuxt.js or Next.js)(予定。後で利用履歴管理画面を作ろうかなて思ってます）

■ DB

-   Sqlite

■ クラウド

-   Back4App

■ その他<br>

-   docker | docker-compose
-   HotPepperAPI
-   Line Messaging API

## 前提

1. HotPepperAPIのキーを取得済み
2. Line公式アカウント、Line Developerアカウント開設済み
3. デバッグするだけならLine Messaging APIの機能以外なら試せます

## 実行方法(WindowsOSでWSL上で動かす想定)

1. .NETSDKのインストール(Windows端末で)

  https://dotnet.microsoft.com/ja-jp/download

2. 証明書の発行(Windows端末で)
  ```
      cd /API
      Dev-Certs-Link.bat
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

6. 確認

  https://localhost:5001/swagger/index.html


