using MoonSharp.Interpreter;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using LuaTest;

namespace MoonSharp_LuaTest
{
    [MoonSharpUserData]
    public class AsyncHandle
    {
        private bool _isDone = false;
        private object _result = null;
        private string _error = null;
        private double _progress = 0.0;
        private bool _cancelled = false;

        private readonly Script _script;

        public AsyncHandle(Script script)
        {
            _script = script;
        }

        public void Start(string taskName, string argument)
        {
            Task.Run(() => ExecuteAsync(taskName, argument));
        }

        private async Task ExecuteAsync(string taskName, string argument)
        {
            try
            {
                if (taskName == "DownloadFile")
                {
                    _progress = 0.1;
                    ManualClockWait();

                    using var client = new HttpClient();
                    var response = await client.GetAsync(argument);

                    ManualClockWait();
                    response.EnsureSuccessStatusCode();

                    var data = await response.Content.ReadAsStringAsync();
                    ManualClockWait();

                    _result = data;
                    _error = null;
                }
                else
                {
                    _error = "Unknown task: " + taskName;
                }
            }
            catch (Exception ex)
            {
                _error = ex.Message;
            }
            finally
            {
                _isDone = true;
            }
        }

        public bool IsDone()
        {
            ManualClockWait();
            return _isDone;
        }

        public DynValue GetResult()
        {
            ManualClockWait();
            return DynValue.FromObject(_script, _result);
        }

        public string GetError()
        {
            ManualClockWait();
            return _error;
        }

        public double GetProgress()
        {
            ManualClockWait();
            return _progress;
        }

        public void Cancel()
        {
            ManualClockWait();
            _cancelled = true;
        }

        private void ManualClockWait()
        {
            if (Form1.LuaFlag)
            {
                if (Form1.ManualClockFlag)
                {
                    while (Form1.ManualClockFlag) { }

                    if (Form1.AutoCmdClockFlag)
                        Form1.ManualClockFlag = true;
                }
            }
            else
            {
                throw new Exception("Lua execution has been stopped.");
            }
        }

        internal static AsyncHandle Run(Func<DynValue> p, DynValue callback)
        {
            throw new NotImplementedException();
        }
    }

    [MoonSharpUserData]
    public class AsyncAPI
    {
        private readonly Script _script;

        public AsyncAPI(Script script)
        {
            _script = script;
        }

        public static AsyncHandle StartAsyncTask(string taskName, DynValue funcOrName, DynValue arg, DynValue callback)
        {
            if (taskName == "LuaFunction")
            {
                return AsyncHandle.Run(() =>
                {
                    try
                    {
                        var result = funcOrName.Function.Call(arg);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        return DynValue.NewTuple(DynValue.Nil, DynValue.NewString(ex.Message));
                    }
                }, callback);
            }

            throw new Exception("未定義のタスク: " + taskName);
        }


    }
}
