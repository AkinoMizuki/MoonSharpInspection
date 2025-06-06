# MoonSharpInspection
C#(.NET 5)でのMoonSharp(ver2.0.0)の実装メモです。

## print(value)
GUI中央の黒いTextBoxに表示できます  
※日本語非対応
```lua
print("文字表示")
```

## WinForm API

### WinForm.sleep(ms)
ミリ秒単位で処理を停止します。
```lua
WinForm.sleep(100)
```

### WinForm.LED(on_off)
フォームのLED表示のON/OFFを切り替えます。
```lua
WinForm.LED(true)
```

## HTTP API

HTTPリクエストを送信したり、Ping確認ができます。

```lua
local req = http.SendRequest("https://httpbin.org/get", "GET")
while not req.IsDone(req) do
    WinForm.sleep(100)
end
if not req.HasError(req) then
    print(req.GetResponseText(req))
end
```

### http
- `http.SendRequest(url, method[, headers, body])`: HTTPリクエスト送信
- `http.Ping(ip)`: Ping確認

### http_request
- `IsDone()`: 完了チェック
- `HasError()`: エラーチェック
- `GetError()`: エラー文字列
- `GetResponseCode()`: ステータスコード
- `GetResponseText()`: 応答テキスト
- `GetResponseData()`: 応答データ（文字列またはバイト配列）

## MySQL API

MySQLに接続し、SQLの実行やクエリ取得を行えます。

```lua
local ok, err = mysql.Connect("Server=localhost;User Id=test;Password=test;")
if ok then
    mysql.ExecuteNonQuery("CREATE DATABASE test_mysql")
    mysql.Disconnect()
end
```

### mysql
- `Connect(connectionString)`: 接続
- `Disconnect()`: 切断
- `IsConnected()`: 接続状態
- `ExecuteQuery(sql)`: クエリ実行
- `ExecuteNonQuery(sql)`: 更新SQL
- `ExecuteScalar(sql)`: 単一値取得
- `BeginTransaction()` / `Commit()` / `Rollback()`: トランザクション

## DateTime API

日時取得、加算、差分、比較などが可能です。

```lua
local now = DateTime.Now()
print("現在:", now.ToString("%Y/%m/%d %H:%M:%S"))
print("年:", now.Year)
print("30秒後:", now.AddSeconds(30).ToString())
```

### DateTime
- `Now()` / `UtcNow()`: 現在時刻
- `FromUnixTime(sec)`: UNIX秒から変換
- `Parse(str)`: 文字列から変換
- `New(y, m, d[, h, m, s])`: 日時生成

### DateTimeObject
- `ToString(format?)`: 書式付き文字列
- `ToUnixTime()`: UNIX秒へ
- `AddSeconds(sec)` / `AddDays(days)`
- `Subtract(other)`: 差分
- `CompareTo(other)`: 比較
- `Year`, `Month`, `Day`, `Hour`, `Minute`, `Second`: 各種プロパティ

### TimeSpanObject
- `TotalSeconds`, `TotalMilliseconds`: 経過時間

## ファイル API（file）

ファイルの読み書き、削除、コピー、移動、サイズ確認などを行えます。

```lua
file.WriteAllText("test.txt", "Hello")
file.AppendAllText("test.txt", "\n追加")
print(file.ReadAllText("test.txt"))
print("サイズ:", file.GetSize("test.txt"))
file.Delete("test.txt")
```

- `WriteAllText(path, text)`
- `AppendAllText(path, text)`
- `ReadAllText(path)`
- `Delete(path)`
- `Copy(src, dest, overwrite?)`
- `Move(src, dest)`
- `GetSize(path)`
- `Exists(path)`

## ディレクトリ API（dir）

フォルダーの作成、削除、存在確認、ファイル一覧取得などが可能です。

```lua
if not dir.Exists("testdir") then
    dir.Create("testdir")
end
local files = dir.GetFiles(".")
for _, name in ipairs(files) do
    print("File:", name)
end
```

- `Exists(path)`
- `Create(path)`
- `Delete(path, recursive?)`
- `GetFiles(path)`
- `GetDirectories(path)`
