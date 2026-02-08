# Agent Tags

依頼時に付けるタグの意味を固定する。

- `#api`: API 実装変更
- `#db`: DB スキーマ/接続設定変更
- `#migration`: Migration 追加/削除/更新
- `#config`: 設定ファイル変更
- `#test`: テスト追加/修正
- `#docs`: ドキュメント更新
- `#refactor`: 振る舞いを変えない整理
- `#breaking`: 破壊的変更を含む
- `#no-test-change`: テストコードは変更しない
- `#safe`: 非破壊・最小差分優先

## 依頼例

`#api #db #migration #no-test-change 目的: PostgreSQL対応`

