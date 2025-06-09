ApiTestModel = {}

local function safe(label, func)
    local ok, result = pcall(func)
    if ok then
        print(label .. ":\t" .. tostring(result))
        return result
    else
        print(label .. ": Error")
        return nil
    end
end

-- Optional helper to wrap object for dot-style method calls
local function wrap(o)
    local mt = {
        __index = function(_, k)
            return function(...) return o[k](o, ...) end
        end
    }
    return setmetatable({}, mt)
end


-- WinForm
ApiTestModel.Test_WinForm = function()
    WinForm.LED(true)
    WinForm.sleep(500)
    WinForm.LED(false)
    WinForm.sleep(500)
end

-- HTTP
ApiTestModel.Test_Http = function()
    print("=== http_request GET API 動作確認 ===")

    local url = "https://httpbin.org/get"
    local method = "GET"

    local req = nil
    local ok, err = pcall(function()
        req = http.SendRequest(url, method)
    end)

    if not ok or req == nil then
        print("SendRequest: エラーでリクエスト開始できません")
        return
    else
        print("SendRequest: OK")
    end

    local maxWait = 50
    local i = 0
    while not req.IsDone(req) and i < maxWait do
        WinForm.sleep(100)
        i = i + 1
    end

    local isDone = false
    ok = pcall(function() isDone = req.IsDone(req) end)
    if ok and isDone then
        print("IsDone: OK")
    else
        print("IsDone: Error or false")
    end

    local hasError = false
    ok = pcall(function() hasError = req.HasError(req) end)
    if not ok then
        print("HasError: Error")
    elseif hasError then
        print("HasError: true")
    else
        print("HasError: false")
    end

    ok, err = pcall(function()
        local errMsg = req.GetError(req)
        if errMsg and errMsg ~= "" then
            print("GetError: " .. errMsg)
        else
            print("GetError: なし")
        end
    end)
    if not ok then
        print("GetError: 呼び出しエラー")
    end

    ok = pcall(function()
        local code = req.GetResponseCode(req)
        print("GetResponseCode: " .. tostring(code))
    end)
    if not ok then
        print("GetResponseCode: Error")
    end

    ok = pcall(function()
        local text = req.GetResponseText(req)
        if text and #text > 0 then
            print("GetResponseText: OK (" .. math.min(#text, 100) .. " chars)")
            print(text)
        else
            print("GetResponseText: 空文字")
        end
    end)
    if not ok then
        print("GetResponseText: Error")
    end

    ok = pcall(function()
        local data = req.GetResponseData(req)
        if type(data) == "string" then
            print("GetResponseData (string): " .. string.sub(data, 1, 50) .. "...")
        elseif type(data) == "table" then
            print("GetResponseData (byte table): 長さ = " .. #data)
        else
            print("GetResponseData: 不明な型")
        end
    end)
    if not ok then
        print("GetResponseData: 呼び出しエラー")
    end

    local pingIp = "8.8.8.8"
    ok, err = pcall(function()
        local result = http.Ping(pingIp)
        if result then
            print("Ping (" .. pingIp .. "): OK")
        else
            print("Ping (" .. pingIp .. "): NG（応答なし）")
        end
    end)
    if not ok then
        print("Ping: 呼び出しエラー → " .. tostring(err))
    end

    print("=== http_request Get API 動作確認 終了 ===")



    print("=== http_request POST API 動作確認（JSON送受信） ===")

    local url = "https://httpbin.org/post"
    local method = "POST"

    local headers = {
        ["Content-Type"] = "application/json"
    }

    local timestamp = DateTime.Now().ToString(DateTime.Now(), "%Y-%m-%d %H:%M:%S")
    
    local message = {
        sender = "LuaScript",
        message = "Hello from Lua via JSON",
        timestamp = timestamp
    }

    for k, v in pairs(message) do
        print(k, type(v), tostring(v))
    end

    local body = json.encode(message)

    local req = nil
    local ok, err = pcall(function()
        req = http.SendRequest(url, method, headers, body)
    end)

    if not ok or req == nil then
        print("SendRequest: エラーでリクエスト開始できません")
        return
    else
        print("SendRequest: OK")
    end

    local maxWait = 50
    local i = 0
    while not req.IsDone(req) and i < maxWait do
        WinForm.sleep(100)
        i = i + 1
    end

    if req.HasError(req) then
        print("エラー:", req.GetError(req))
        return
    else
        print("HTTP Status:", req.GetResponseCode(req))
    end

    local text = req.GetResponseText(req)
    print("受信テキスト:\n" .. text)

    local decoded = nil
    ok, err = pcall(function()
        decoded = json.decode(text)
    end)

    if not ok or not decoded then
        print("[ERROR] JSONのパースに失敗: " .. tostring(err))
        return
    end

    if decoded.json then
        print("== 受信JSON 内容 ==")
        for k, v in pairs(decoded.json) do
            print(k .. ":", v)
        end
    else
        print("[INFO] jsonフィールドが含まれていません")
    end

    print("=== http_request API 動作確認 終了 ===")
end

-- MySQL
ApiTestModel.Test_MySQL = function(ip, user, pass, db)

    print("=== MySQL API テスト開始 ===")

    -- データベース作成のための管理接続
    local connStrCreate = string.format("Server=%s;User Id=%s;Password=%s;", ip, user, pass)
    local ok, err = mysql.Connect(connStrCreate)
    if not ok then
        print("[ERROR] 接続失敗（DB未指定）: " .. tostring(err))
        return
    end

    print("MySQL接続成功（管理接続）")

    -- データベース作成（すでにある場合は無視）
    local sqlCreate = "CREATE DATABASE IF NOT EXISTS " .. db
    mysql.ExecuteNonQuery(sqlCreate)

    -- testユーザー作成とtesrtへのアクセス権（任意: すでに設定済みならスキップ可能）
    mysql.ExecuteNonQuery("CREATE USER IF NOT EXISTS 'test'@'%' IDENTIFIED BY 'test'")
    mysql.ExecuteNonQuery("GRANT ALL PRIVILEGES ON " .. db .. ".* TO 'test'@'%'")
    mysql.ExecuteNonQuery("FLUSH PRIVILEGES")

    mysql.Disconnect()

    -- 本番接続（データベース指定あり）
    local connStr = string.format("Server=%s;Database=%s;User Id=%s;Password=%s;", ip, db, "test", "test")
    ok, err = mysql.Connect(connStr)
    if not ok then
        print("[ERROR] DB接続失敗: " .. tostring(err))
        return
    end

    print("MySQL接続成功（DB: " .. db .. "）")

    -- テーブル作成
    mysql.ExecuteNonQuery([[
        CREATE TABLE IF NOT EXISTS sample_table (
            id INT AUTO_INCREMENT PRIMARY KEY,
            message TEXT
        )
    ]])

    -- データ挿入
    mysql.ExecuteNonQuery("INSERT INTO sample_table (message) VALUES ('Hello from Lua!')")

    -- データ取得
    local rows, err = mysql.ExecuteQuery("SELECT id, message FROM sample_table")
    if rows then
        for _, row in ipairs(rows) do
            print("Row:", row.id, row.message)
        end
    else
        print("[ERROR] SELECT 失敗: " .. tostring(err))
    end

    mysql.Disconnect()

    -- 削除処理のために管理接続再び
    ok, err = mysql.Connect(connStrCreate)
    if not ok then
        print("[ERROR] 再接続失敗（削除処理）: " .. tostring(err))
        return
    end

    -- テスト用ユーザーとDB削除
    mysql.ExecuteNonQuery("DROP USER IF EXISTS 'test'@'%'")
    mysql.ExecuteNonQuery("DROP DATABASE IF EXISTS " .. db)
    mysql.ExecuteNonQuery("FLUSH PRIVILEGES")

    mysql.Disconnect()

    print("=== MySQL API テスト終了 ===")

end

-- MyLuaDownload
function MyLuaDownload(url)
    print("[Lua] MyLuaDownload 実行開始: " .. url)

    local req = http.SendRequest(url, "GET")
    local i = 0
    while not req.IsDone(req) and i < 50 do
        WinForm.sleep(100)
        i = i + 1
    end

    if req.HasError(req) then
        return nil, req.GetError(req)
    else
        return req.GetResponseText(req)
    end
end


-- File/Directory 完全テスト
ApiTestModel.Test_FileFull = function(folderPath)

    print("=== File/Directory API テスト開始 ===")

    -- パス整形（末尾に "/" があるか確認）
    local path = folderPath:gsub("\\", "/")
    if not path:match("/$") then
        path = path .. "/"
    end
    print("整形済みフォルダーパス:", path)

    -- 存在チェックと作成
    if not dir.Exists(path) then
        print("フォルダーが存在しません。作成します...")
        if dir.Create(path) then
            print("フォルダー作成成功:", path)
        else
            print("[ERROR] フォルダー作成失敗")
            return
        end
    else
        print("フォルダーはすでに存在しています")
    end

    if not dir.Exists(path) then
        print("[ERROR] フォルダー確認失敗")
        return
    else
        print("フォルダー存在確認: OK")
    end

    -- ファイルテスト
    local filepath = path .. "testfile.txt"
    if file.WriteAllText(filepath, "Hello File\n") then
        print("WriteAllText: OK")
    else
        print("[ERROR] WriteAllText 失敗")
    end

    if file.AppendAllText(filepath, "追加の行\n") then
        print("AppendAllText: OK")
    else
        print("[ERROR] AppendAllText 失敗")
    end

    -- ファイル作成と追記
    file.WriteAllText(filepath, "Hello File\n")
    file.AppendAllText(filepath, string.rep("追加の行\n", 50))  -- 50行分
    
    -- ファイル読み込みとサイズ確認
    local content = file.ReadAllText(filepath)
    if content then
        print("ReadAllText: OK")
        print("内容:\n" .. content)
        
        -- filepath を指定してサイズ取得
        local size = file.GetSize(filepath)
        print("GetSize:", size)
    else
        print("[ERROR] ReadAllText 失敗")
    end

    if file.Delete(filepath) then
        print("Delete: OK")
    else
        print("[ERROR] Delete 失敗")
    end

    if not file.Exists(filepath) then
        print("削除確認: OK")
    else
        print("[ERROR] ファイルが残っています")
    end

    -- コピー処理
    local copyPath = path .. "copyfile.txt"
    local movedPath = path .. "movedfile.txt"
    
    file.WriteAllText(copyPath, "Copy Test\n")
    if file.Copy(copyPath, movedPath, true) then
        print("Copy: OK")
    else
        print("[ERROR] Copy 失敗")
    end
    
    -- 移動処理の前に既存ファイルがあれば削除
    if file.Exists(movedPath) then
        file.Delete(movedPath)
    end
    
    if file.Move(copyPath, movedPath) then
        print("Move: OK")
    else
        print("[ERROR] Move 失敗")
    end

    local size = file.GetSize(copyPath)
    print("GetSize:", size)

    -- ファイル一覧
    local files = dir.GetFiles(path)
    print("現在のファイル一覧:")
    for _, f in ipairs(files) do
        print(" -", f)
    end

    -- 使用ファイル削除
    file.Delete(copyPath)

    -- 最後にフォルダー自体を削除
    if dir.Delete(path, true) then
        print("フォルダー削除: OK")
    else
        print("[ERROR] フォルダー削除失敗")
    end

    print("=== File/Directory API テスト終了 ===")

end

-- DateTime
ApiTestModel.Test_DateTime = function()

    print("=== DateTime API 動作確認（新形式） ===")

    local function safe(label, func)
        local ok, result = pcall(func)
        if ok then
            print(label .. ":", result)
            return result
        else
            print(label .. ": Error")
            return nil
        end
    end

    -- Now / UtcNow
    local now = safe("Now", function() return DateTime.Now().ToString("%Y-%m-%d %H:%M:%S") end)
    local utc = safe("UtcNow", function() return DateTime.UtcNow().ToString("%Y-%m-%d %H:%M:%S") end)

    -- 個別プロパティ確認
    local nowObj = DateTime.Now()
    print("Year:", nowObj.Year)
    print("Month:", nowObj.Month)
    print("Day:", nowObj.Day)
    print("Hour:", nowObj.Hour)
    print("Minute:", nowObj.Minute)
    print("Second:", nowObj.Second)

    -- ToUnixTime
    safe("ToUnixTime", function() return nowObj:ToUnixTime() end)

    -- FromUnixTime
    local unix = nowObj:ToUnixTime()
    safe("FromUnixTime", function() return DateTime.FromUnixTime(unix).ToString("%Y/%m/%d %H:%M:%S") end)

    -- AddSeconds
    safe("AddSeconds(+30)", function() return nowObj:AddSeconds(30).ToString("%Y-%m-%d %H:%M:%S") end)

    -- AddDays
    safe("AddDays(+1)", function() return nowObj:AddDays(1).ToString("%Y-%m-%d") end)

    -- Subtract（差分計算）
    local tomorrow = nowObj:AddDays(1)
    safe("Subtract (tomorrow - now)", function()
        local span = tomorrow:Subtract(nowObj)
        return string.format("Sec: %.0f, mSec: %.0f", span.TotalSeconds, span.TotalMilliseconds)
    end)

    -- CompareTo
    safe("CompareTo(now vs tomorrow)", function() return nowObj:CompareTo(tomorrow) end)
    safe("CompareTo(now vs now)", function() return nowObj:CompareTo(nowObj) end)
    safe("CompareTo(tomorrow vs now)", function() return tomorrow:CompareTo(nowObj) end)

    -- New()
    safe("New(2025, 5, 21, 15, 30, 45)", function()
        return DateTime.New(2025, 5, 21, 15, 30, 45).ToString("%Y-%m-%d %H:%M:%S")
    end)

    -- Parse()
    safe("Parse(\"2025-12-31 23:59:59\")", function()
        return DateTime.Parse("2025-12-31 23:59:59").ToString("%Y/%m/%d %H:%M:%S")
    end)

    print("=== DateTime API 動作確認 終了 ===")

end
return ApiTestModel
