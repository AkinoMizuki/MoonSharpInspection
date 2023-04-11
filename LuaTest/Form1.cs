using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using static MoonSharp_LuaTest.MoonSharpFunction;

namespace LuaTest
{
    public partial class Form1 : Form
    {

        public static bool LuaFlag { get; set; } = false;
        public static bool ManualClockFlag { get; set; } = false;
        public static bool AutoCmdClockFlag { get; set; } = false;
        public static string Receive { get; set; } =  "";

        //LED制御
        public static Color On_LED { get; set; } = Color.Lime;
        public static Color Off_LED { get; set; } = Color.Gray;

        public Form1()
        {
            InitializeComponent();
        }

        /************************************************************/
        /*                  フォーム関数                            */
        /************************************************************/
        private void LuaLeadButtton_Click(object sender, EventArgs e)
        {/*=== LuaFileセット===*/

            openFileDialog1.FileName = "man.lua";   //既定のファイル名
            openFileDialog1.DefaultExt = ".lua";    //既定のファイル拡張子
            //openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog1.InitialDirectory = Application.StartupPath + "main_Lua";
            openFileDialog1.Filter = "Lua failes *Lua |*.lua";

            //ダイアログ表示
            if(openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //指定されたFileをテキストに表示する
                LuaTextBox.Text = openFileDialog1.FileName;
                //文字入力位置(キャレット)を末尾に設定する
                LuaTextBox.SelectionStart = LuaTextBox.Text.Length;

            }

        }/*=== END_LuaFileセット===*/
        private void LuaTextBox_DragEnter(object sender, DragEventArgs e)
        {
            //ファイルがドラッグされている場合、カーソルを変更する
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void LuaTextBox_DragDrop(object sender, DragEventArgs e)
        {
            //ドロップされたファイルの一覧を取得
            string[] fileName = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if(fileName.Length <= 0)
            {
                return;
            }

            //ドロップ先がテキストボックスであるかチェック
            TextBox LuaTextBox = sender as TextBox;
            if(LuaTextBox == null)
            {
                return;
            }

            //テキストボックスの内容をファイル名に変更
            LuaTextBox.Text = fileName[0];

            LuaTextBox.Select(LuaTextBox.Text.Length, 0);

        }

        private void StartLuaButton_Click(object sender, EventArgs e)
        {/*=== Luaスタート===*/

            if(LuaTextBox.Text != "")
            {/*=== 空チェック === */

                //フォーム制御
                StartLuaButton.Enabled = false;
                LuaLeadButtton.Enabled = false;
                LuaTextBox.Enabled = false;

                ManualClockCheckBox.Enabled = false;
                if(ManualClockCheckBox.Checked == true)
                {
                    TapCmdClockButton.Enabled = true;
                    AutoCmdClockButton.Enabled = true;
                    AutoCmdClockFlag = true;
                    ManualClockFlag = true;
                }

                //マニュアルClock用
                LuaFlag = true;

                //開始メッセージ
                LogTextBox.AppendText("=== Start Lua Cmd ===" + Environment.NewLine);
                Task task = new Task(() =>
                {//別タスク処理

                    //Luaスタート
                    MainLua(LuaTextBox.Text);

                    LuaFlag = false;
                    this.Invoke((Action)(() =>
                    {//別タスクからUI制御します

                        //フォーム制御
                        StartLuaButton.Enabled = true;
                        LuaLeadButtton.Enabled = true;
                        LuaTextBox.Enabled = true;
                        ManualClockCheckBox.Enabled = true;
                        TapCmdClockButton.Enabled = false;
                        AutoCmdClockButton.Enabled = false;
                        AutoCmdClockFlag = false;
                        LogTextBox.AppendText("=== END Lua Cmd ===" + Environment.NewLine);


                    }));//EDN_別タスクからUI制御します

                    GC.Collect();//アクセス不可能なオブジェクトを除去
                    GC.WaitForPendingFinalizers();//ファイナライゼーションが終わるまでスレット待機
                    GC.Collect();//ファイナライズされたばかりのオブジェクトに関するメモリを開放

                });//END_別タスク処理
                task.Start();

            }/*=== END_空チェック === */

        }/*=== END_Luaスタート===*/

        private void StopLuaButton_Click(object sender, EventArgs e)
        {/*=== Lua停止 ===*/

            if(LuaFlag == true)
            {
                //フォーム制御
                StartLuaButton.Enabled = true;
                LuaLeadButtton.Enabled = true;
                LuaTextBox.Enabled = true;
                ManualClockCheckBox.Enabled = false;
                ManualClockCheckBox.Checked = false;
                TapCmdClockButton.Enabled = false;
                AutoCmdClockButton.Enabled = false;
                AutoCmdClockFlag = false;
                ManualClockFlag = false;
                LogTextBox.AppendText("=== END Lua Cmd ===" + Environment.NewLine);
                LuaFlag = false;

            }

        }/*=== END_Lua停止 ===*/

        private void TapCmdClockButton_Click(object sender, EventArgs e)
        {/*=== TapCmdClock ===*/
            ManualClockFlag = false;
        }/*=== END_TapCmdClock ===*/

        private void AutoCmdClockButton_Click(object sender, EventArgs e)
        {/*=== AutoCmdClock ===*/

            AutoCmdClockFlag = false;
            ManualClockFlag = false;
            ManualClockCheckBox.Enabled = false;
            TapCmdClockButton.Enabled = false;
            AutoCmdClockButton.Enabled = false;

        }/*=== END_AutoCmdClock ===*/

        private void ClearButton_Click(object sender, EventArgs e)
        {/*=== ClearButton ===*/
            LogTextBox.Clear();
        }/*=== End_ClearButton ===*/

        /************************************************************/
        /*                  Luaスタート                             */
        /************************************************************/
        public void MainLua(string FullPass)
        {/*=== コンストラクタ ===*/

            if(FullPass != "")
            {/*=== 空チェック ===*/

                /*=== スクリプトのパス取得 ===*/
                string Pass = FullPass.Substring(FullPass.LastIndexOf("\\") + 1);
                string Address = FullPass.Replace(Pass,"");

                if(Directory.Exists(Address))
                {/*=== 保存フォルダーの確認 ===*/

                    //テキストとしてLuaを呼び出し
                    string lines = File.ReadAllText(FullPass);

                    try
                    {
                        //スクリプト定義
                        Script script = new Script();


                        ((ScriptLoaderBase)script.Options.ScriptLoader).ModulePaths = new[]
                        {/*=== モジュール呼び出し ===*/

                            Path.Combine(Address, "?"),
                            Path.Combine(Address, "?.lua")

                        };/*=== END_モジュール呼び出し ===*/


                        script.Options.DebugPrint = msg =>
                        {/*=== printコマンド ===*/

                            if (LuaFlag == true)
                            {//スキップ用

                                if (ManualClockFlag == true)
                                {//マニュアルクロック用

                                    while (ManualClockFlag == true) { }

                                    if (AutoCmdClockFlag == true)
                                    {
                                        ManualClockFlag = true;
                                    }

                                }//END_マニュアルクロック用

                                this.Invoke((Action)(() =>
                                {//別のスレットでUIを制御します
                                    LogTextBox.AppendText(msg + Environment.NewLine);
                                }));//END_別のスレットでUIを制御します

                            }//スキップ用

                        };/*=== END_printコマンド ===*/

                        script.Globals["printf"] = (Func<object, string>)Printf;
                        //WinFormクラスをスクリプト内で使えるようにする
                        UserData.RegisterAssembly(typeof(WinForm).Assembly);
                        //WinFormクラスをスクリプト内部で使えるようにする
                        script.Globals["WinForm"] = new WinForm(this);


                        //Luaスタート
                        DynValue function = script.DoString(lines);


                    }
                    catch (MoonSharp.Interpreter.ScriptRuntimeException ex)
                    {/*=== コードエラー ===*/

                        this.Invoke((Action)(() =>
                        {//別のスレットでUIを制御します
                            LogTextBox.AppendText("Code error => " + ex.DecoratedMessage + Environment.NewLine);
                        }));//END_別のスレットでUIを制御します

                    }/*=== END_コードエラー ===*/
                    catch (MoonSharp.Interpreter.SyntaxErrorException ex)
                    {/*=== コンパイルエラー ===*/

                        this.Invoke((Action)(() =>
                        {//別のスレットでUIを制御します
                            LogTextBox.AppendText("Compile error => " + ex.DecoratedMessage + Environment.NewLine);
                        }));//END_別のスレットでUIを制御します

                    }/*=== END_コンパイルエラー ===*/
                    catch (Exception ex)
                    {/*=== 未知のエラー ===*/

                        this.Invoke((Action)(() =>
                        {//別のスレットでUIを制御します
                            LogTextBox.AppendText("Error => " + ex.Message + Environment.NewLine);
                        }));//END_別のスレットでUIを制御します

                    }/*=== END_未知のエラー ===*/


                }/*=== END_保存フォルダーの確認 ===*/


            }/*=== END_空チェック ===*/

        }/*=== END_コンストラクタ ===*/



        /************************************************************/
        /*               　MoonSharp_関数クラス                     */
        /************************************************************/
        string Printf(object obj)
        {

            {//スキップ用

                if (ManualClockFlag == true)
                {//マニュアルクロック用

                    while (ManualClockFlag == true) { }

                    if (AutoCmdClockFlag == true)
                    {
                        ManualClockFlag = true;
                    }

                }//END_マニュアルクロック用

                this.Invoke((Action)(() =>
                {//別のスレットでUIを制御します

                    LogTextBox.Text = (obj + Environment.NewLine);
                    LogTextBox.Select(LogTextBox.Text.Length, 0);

                }));//END_別のスレットでUIを制御します

            }//スキップ用

            return (string)obj;

        }

    }
}
