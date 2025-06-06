using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Text;
using LuaTest;

namespace MoonSharp_LuaTest
{
    [MoonSharpUserData]
    public class HttpRequest
    {
        private bool _isDone = false;
        private int _responseCode = 200;
        private string _responseText = "OK";
        private byte[] _responseData = Encoding.UTF8.GetBytes("OK");
        private bool _hasError = false;
        private string _errorMessage = "";

        public bool IsDone()
        {
            return _isDone;
        }

        public bool Ping(string ip)
        {
            ManualClockWait(); // ← マニュアルクロック対応
            return !string.IsNullOrEmpty(ip);
        }

        public int GetResponseCode()
        {
            ManualClockWait();
            return _responseCode;
        }

        public string GetResponseText()
        {
            ManualClockWait();
            return _responseText;
        }

        public DynValue GetResponseData(Script script)
        {
            ManualClockWait();
            var table = new Table(script);
            for (int i = 0; i < _responseData.Length; i++)
            {
                table[i + 1] = DynValue.NewNumber(_responseData[i]);
            }
            return DynValue.NewTable(table);
        }

        public bool HasError()
        {
            ManualClockWait();
            return _hasError;
        }

        public string GetError()
        {
            ManualClockWait();
            return _errorMessage;
        }

        public void SimulateResponse(string text, int code = 200)
        {
            _isDone = true;
            _responseCode = code;
            _responseText = text;
            _responseData = Encoding.UTF8.GetBytes(text);
        }

        public void SimulateError(string error)
        {
            _isDone = true;
            _hasError = true;
            _errorMessage = error;
        }

        private void ManualClockWait()
        {
            if (Form1.LuaFlag == true)
            {
                if (Form1.ManualClockFlag == true)
                {
                    while (Form1.ManualClockFlag == true) { }

                    if (Form1.AutoCmdClockFlag == true)
                    {
                        Form1.ManualClockFlag = true;
                    }
                }
            }
            else
            {
                throw new Exception("Lua execution has been stopped.");
            }
        }
    }

    [MoonSharpUserData]
    public class Http
    {
        public HttpRequest SendRequest(string url, string method, Table headers = null, string body = null)
        {
            var request = new HttpRequest();
            request.SimulateResponse($"Method: {method}, URL: {url}");
            return request;
        }

        public bool Ping(string ip)
        {
            // 同様にマニュアルクロックを挿入してもよい
            return !string.IsNullOrEmpty(ip);
        }
    }
}
