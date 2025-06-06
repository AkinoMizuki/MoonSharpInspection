--モジュール
local module_1 = require ("module_1")
local module_2 = {}


module_2.moduleTest ={}

module_2.test = function ()
	local domo = module_1.test2() + module_1.test3()
	return domo
end

module_2.moduleTest.hoge = function ()
	print("module_2 to hoge")
end

--END_モジュール
return module_2