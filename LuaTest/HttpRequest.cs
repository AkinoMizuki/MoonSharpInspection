using LuaTest;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MoonSharp_LuaTest
{
    [MoonSharpUserData]
    public class HttpRequest
    {
        private bool _isDone = false;
        private int _responseCode = 0;
        private string _responseText = "";
        private byte[] _responseData = new byte[0];
        private bool _hasError = false;
        private string _errorMessage = "";

        public bool IsDone()
        {
            return _isDone;
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

        internal async Task StartRequestAsync(string url, string method, Table headers = null, string body = null)
        {
            try
            {
                using var client = new HttpClient();
                using var reqMessage = new HttpRequestMessage(new HttpMethod(method), url);

                // ヘッダー処理
                if (headers != null)
                {
                    foreach (var pair in headers.Pairs)
                    {
                        var key = pair.Key.CastToString();
                        var val = pair.Value.CastToString();
                        reqMessage.Headers.TryAddWithoutValidation(key, val);
                    }
                }

                // ボディがある場合
                if (!string.IsNullOrEmpty(body))
                {
                    reqMessage.Content = new StringContent(body, Encoding.UTF8, "application/json");
                }

                var response = await client.SendAsync(reqMessage);
                _responseCode = (int)response.StatusCode;
                _responseText = await response.Content.ReadAsStringAsync();
                _responseData = Encoding.UTF8.GetBytes(_responseText);
            }
            catch (Exception ex)
            {
                _hasError = true;
                _errorMessage = ex.Message;
            }
            finally
            {
                _isDone = true;
            }
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
            var req = new HttpRequest();
            // 非同期実行して Lua 側では IsDone() で完了を確認
            _ = req.StartRequestAsync(url, method, headers, body);
            return req;
        }

        public bool Ping(string ip)
        {
            // 通信を使わないダミー
            return !string.IsNullOrEmpty(ip);
        }
    }
}
