using MoonSharp.Interpreter;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using LuaTest;

namespace MoonSharp_LuaTest
{
    [MoonSharpUserData]
    public class MySQL
    {
        private static MySqlConnection _conn;
        private static MySqlTransaction _tran;

        private static void ManualClockWait()
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

        public static DynValue Connect(string connectionString)
        {
            ManualClockWait();

            try
            {
                _conn = new MySqlConnection(connectionString);
                _conn.Open();
                return DynValue.NewTuple(DynValue.NewBoolean(true), DynValue.Nil);
            }
            catch (Exception ex)
            {
                return DynValue.NewTuple(DynValue.NewBoolean(false), DynValue.NewString(ex.Message));
            }
        }

        public static bool Disconnect()
        {
            ManualClockWait();

            try
            {
                _conn?.Close();
                _conn = null;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsConnected()
        {
            ManualClockWait();
            return _conn != null && _conn.State == ConnectionState.Open;
        }

        public static DynValue ExecuteNonQuery(string sql)
        {
            ManualClockWait();

            try
            {
                using var cmd = new MySqlCommand(sql, _conn, _tran);
                int result = cmd.ExecuteNonQuery();
                return DynValue.NewTuple(DynValue.NewNumber(result), DynValue.Nil);
            }
            catch (Exception ex)
            {
                return DynValue.NewTuple(DynValue.NewNumber(-1), DynValue.NewString(ex.Message));
            }
        }

        public static DynValue ExecuteScalar(string sql)
        {
            ManualClockWait();

            try
            {
                using var cmd = new MySqlCommand(sql, _conn, _tran);
                object result = cmd.ExecuteScalar();
                return DynValue.NewTuple(DynValue.FromObject(null, result), DynValue.Nil);
            }
            catch (Exception ex)
            {
                return DynValue.NewTuple(DynValue.Nil, DynValue.NewString(ex.Message));
            }
        }

        public static DynValue ExecuteQuery(Script script, string sql)
        {
            ManualClockWait();

            try
            {
                using var cmd = new MySqlCommand(sql, _conn, _tran);
                using var reader = cmd.ExecuteReader();

                var resultTable = new Table(script);
                int rowIndex = 1;

                while (reader.Read())
                {
                    var row = new Table(script);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = DynValue.FromObject(script, reader.GetValue(i));
                    }
                    resultTable[rowIndex++] = DynValue.NewTable(row);
                }

                return DynValue.NewTuple(DynValue.NewTable(resultTable), DynValue.Nil);
            }
            catch (Exception ex)
            {
                return DynValue.NewTuple(DynValue.Nil, DynValue.NewString(ex.Message));
            }
        }

        public static bool BeginTransaction()
        {
            ManualClockWait();

            try
            {
                _tran = _conn.BeginTransaction();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Commit()
        {
            ManualClockWait();

            try
            {
                _tran?.Commit();
                _tran = null;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Rollback()
        {
            ManualClockWait();

            try
            {
                _tran?.Rollback();
                _tran = null;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
