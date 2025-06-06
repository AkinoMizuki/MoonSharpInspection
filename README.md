# MoonSharpInspection
C#(.NET 5)でのMoonSharp(ver2.0.0)の実装メモです。

# 実装API
- print(value)
- WinForm.sleep(msec)
- WinForm.LED(boolean)

# print(value)
GUI中央の黒いTextBoxに表示できます
※日本語非対応
```lua
  print("文字表示")
```

# WinForm.sleep(msec)
msecオーダーでウェイトを指定出来ます
```lua
 --100msウェイト
　WinForm.sleep(100)
```

# WinForm.LED(on_off)
フォームアプリケーションのLEDピクチャをONもしくはOFFできます
```lua
 --Onします
　WinForm.LED(true)
```

# HTTP API

### http.SendRequest(url, method[, headers, body])
HTTPリクエストを送信します。戻り値は `http_request` オブジェクトです。
```lua
local req = http.SendRequest("https://example.com", "GET")
```

### http.Ping(ip)
指定IPにPingを送信し、成功すれば `true` を返します。
```lua
local success = http.Ping("8.8.8.8")
```

### http_request オブジェクト
リクエストの状態を取得できます。
```lua
if req:IsDone() then
  if req:HasError() then
    print("エラー:", req:GetError())
  else
    print("コード:", req:GetResponseCode())
    print("本文:", req:GetResponseText())
  end
end
```

# MySQL API

### mysql.Connect(connectionString)
MySQLに接続します。戻り値は `(true/false, エラーメッセージ)`。
```lua
local ok, err = mysql.Connect("Server=localhost;User Id=root;Password=pass;")
```

### mysql.ExecuteQuery(sql)
SQLクエリを実行し、行データのテーブルを返します。
```lua
local rows, err = mysql.ExecuteQuery("SELECT * FROM mytable")
for _, row in ipairs(rows) do
  print(row.id, row.name)
end
```

### mysql.ExecuteNonQuery(sql)
更新系SQLを実行し、影響行数を返します。

### その他
- `IsConnected()`：接続確認
- `Disconnect()`：接続終了
- `BeginTransaction()` / `Commit()` / `Rollback()`：トランザクション制御

# ファイル API（file）

```lua
file.WriteAllText("test.txt", "Hello")
file.AppendAllText("test.txt", "\n追加")
print(file.ReadAllText("test.txt"))
print("サイズ:", file.GetSize("test.txt"))
file.Delete("test.txt")
```

- `Exists(path)`：存在確認
- `Copy(src, dest, overwrite?)`：コピー
- `Move(src, dest)`：移動

# ディレクトリ API（dir）

```lua
if not dir.Exists("testdir") then
  dir.Create("testdir")
end
local files = dir.GetFiles(".")
for _, name in ipairs(files) do
  print("File:", name)
end
```

- `Delete(path, recursive?)`：削除（再帰も可）
- `GetDirectories(path)`：サブディレクトリ取得

# DateTime API

```lua
local now = DateTime.Now()
print("現在:", now:ToString("%Y/%m/%d %H:%M:%S"))
print("年:", now.Year)
print("30秒後:", now:AddSeconds(30):ToString())

local tomorrow = now:AddDays(1)
local span = tomorrow:Subtract(now)
print("差:", span.TotalSeconds, "秒")
```

### 主な関数とプロパティ

- `DateTime.Now()` / `UtcNow()`：現在日時取得
- `FromUnixTime(sec)`：UNIX秒から日時変換
- `Parse(str)`：文字列から日時
- `New(y, m, d[, h, m, s])`：任意日時生成
- `ToUnixTime()`：UNIX秒変換
- `ToString(format?)`：日時文字列
- `AddSeconds(sec)` / `AddDays(days)`
- `Subtract(other)`：時間差（TimeSpanObject）
- `CompareTo(other)`：大小比較
