using System;
using System.IO;
using System.Linq;
using MoonSharp.Interpreter;
using LuaTest;

namespace MoonSharp_LuaTest
{
    [MoonSharpUserData]
    public class FileAPI
    {
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

        public bool Exists(string path)
        {
            ManualClockWait();
            return File.Exists(path);
        }

        public string ReadAllText(string path)
        {
            ManualClockWait();
            return File.ReadAllText(path);
        }

        public bool WriteAllText(string path, string text)
        {
            ManualClockWait();
            File.WriteAllText(path, text);
            return true;
        }

        public bool AppendAllText(string path, string text)
        {
            ManualClockWait();
            File.AppendAllText(path, text);
            return true;
        }

        public bool Delete(string path)
        {
            ManualClockWait();
            File.Delete(path);
            return true;
        }

        public bool Copy(string source, string dest, bool overwrite = false)
        {
            ManualClockWait();
            File.Copy(source, dest, overwrite);
            return true;
        }

        public bool Move(string source, string dest)
        {
            ManualClockWait();
            File.Move(source, dest);
            return true;
        }

        public double GetSize(string path)
        {
            ManualClockWait();
            var fi = new FileInfo(path);
            return fi.Exists ? fi.Length : 0;
        }
    }

    [MoonSharpUserData]
    public class DirAPI
    {
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

        public bool Exists(string path)
        {
            ManualClockWait();
            return Directory.Exists(path);
        }

        public bool Create(string path)
        {
            ManualClockWait();
            Directory.CreateDirectory(path);
            return true;
        }

        public bool Delete(string path, bool recursive = false)
        {
            ManualClockWait();
            Directory.Delete(path, recursive);
            return true;
        }

        public string[] GetFiles(string path)
        {
            ManualClockWait();
            return Directory.GetFiles(path);
        }

        public string[] GetDirectories(string path)
        {
            ManualClockWait();
            return Directory.GetDirectories(path);
        }
    }
}
