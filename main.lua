--モジュール
local module_1 = require ("module_1")
local module_2 = require ("module_2")

---------------------------------------
--		メイン関数
---------------------------------------
function main()

	--簡単な計算
	local int = 1
	int = int + 2
	print("1+2 Result : " .. int)

	--LED_ON
	WinForm.LED(true)

	--関数呼び出し
	for count = 1, 10 do
		print("Count" .. count)
		WinForm.sleep(60)
	end

	--LED_OFF
	WinForm.LED(false)

	print("=== Module ===")
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