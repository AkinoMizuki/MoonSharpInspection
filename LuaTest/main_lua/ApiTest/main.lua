-- グローバルMySQL接続設定
local mysql_server_ip   = "hyoudoukan.info"
local mysql_user        = "test"
local mysql_password    = "test"
local mysql_database    = "tesrt"
local testdir = "main_Lua/testdir/"

-- テスト実行
local ApiTestModel = require ("ApiTestModel")

print("=== API TEST START ===")

print(">> WinForm")
ApiTestModel.Test_WinForm()

print(">> HTTP")
ApiTestModel.Test_Http()

print(">> DateTime")
ApiTestModel.Test_DateTime()

print(">> MySQL")
ApiTestModel.Test_MySQL(mysql_server_ip,mysql_user,mysql_password,mysql_database)

print(">> Test_FileFull")
ApiTestModel.Test_FileFull(testdir)

print("=== API TEST END ===")