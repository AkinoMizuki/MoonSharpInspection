--モジュール
local module_1 = require ("module_1")
local module_2 = require ("module_2")

---------------------------------------
--		メイン関数
---------------------------------------
function main()

	--簡単な計算
	local int = 1
	int = in + 2
	print("1+2 Result : " .. int)

	print(error)
	
	--関数呼び出し
	for count = 1, 10 do
		print("Count" .. count)
		winForm.sleep(60)
	end

	print("=== モジュール関数 ===")
	print(module_1.test())
	print(module_2.test())
	module_2.moduleTest.hoge()

end

---------------------------------------
--		スタート
---------------------------------------
--スタート
main()
--終了確認用
print("Hello Moon Sharp")