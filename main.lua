--���W���[��
local module_1 = require ("module_1")
local module_2 = require ("module_2")

---------------------------------------
--		���C���֐�
---------------------------------------
function main()

	--�ȒP�Ȍv�Z
	local int = 1
	int = int + 2
	print("1+2 Result : " .. int)

	--LED_ON
	WinForm.LED(true)

	--�֐��Ăяo��
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
--		�X�^�[�g
---------------------------------------
--�X�^�[�g
main()
--�I���m�F�p
print("Hello Moon Sharp")

printf("printf ok")