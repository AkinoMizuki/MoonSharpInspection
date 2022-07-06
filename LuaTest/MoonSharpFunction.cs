using LuaTest;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using System;
using System.Threading;

namespace MoonSharp_LuaTest
{
    public partial class MoonSharpFunction
    {

        [MoonSharpUserData]
        public partial class WinForm
        {/*=== MoonSharp_関数クラス ===*/

            public Form1 form1;
            public WinForm(Form1 obj_Form1)
            {/*=== インスタンス引継ぎ ===*/
                form1 = obj_Form1;
            }/*=== END_インスタンス引継ぎ ===*/

            public void sleep(string msec)
            {/*=== スリーブ関数 ===*/

                if(Form1.LuaFlag == true)
                {//スキップ用

                    if(Form1.ManualClockFlag == true)
                    {//マニュアルクロック用

                        while(Form1.ManualClockFlag == true) { }

                        if(Form1.AutoCmdClockFlag == true)
                        {
                            Form1.ManualClockFlag = true;
                        }

                    }//END_マニュアルクロック用

                    Thread.Sleep(int.Parse(msec.ToString()));

                }
                else
                {//タスク破棄用
                    throw new Exception();
                }//END_タスク破棄用

            }/*=== END_スリーブ関数 ===*/

            public void LED(bool msec)
            {/*=== スリーブ関数 ===*/

                if (Form1.LuaFlag == true)
                {//スキップ用

                    if (Form1.ManualClockFlag == true)
                    {//マニュアルクロック用

                        while (Form1.ManualClockFlag == true) { }

                        if (Form1.AutoCmdClockFlag == true)
                        {
                            Form1.ManualClockFlag = true;
                        }

                    }//END_マニュアルクロック用

                    if(msec == true)
                    {/*=== 色変え ===*/

                        form1.LED_1.BackColor = Form1.On_LED;

                    }
                    else
                    {

                        form1.LED_1.BackColor = Form1.Off_LED;

                    }/*=== END_色変え ===*/

                }
                else
                {//タスク破棄用
                    throw new Exception();
                }//END_タスク破棄用

            }/*=== END_スリーブ関数 ===*/

        }/*=== MoonSharp_関数クラス ===*/

    }
}
