--���W���[��
local module_1 = {}

module_1.test = function ()
	local text = "module_1 to test"
	return text
end

module_1.test2 = function ()
	local text2 = 7
	return text2
end

module_1.test3 = function ()
	local text3 = 5
	return text3
end


module_1.test4 = function (Flag)
	return (not Flag)
end

--END_���W���[��
return module_1