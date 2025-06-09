using MoonSharp.Interpreter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MoonSharp_LuaTest
{
    [MoonSharpUserData]
    public class JsonApi
    {
        public string encode(DynValue luaValue)
        {
            try
            {
                var obj = ConvertDynValueToObject(luaValue);
                return JsonConvert.SerializeObject(obj, Formatting.None);
            }
            catch (Exception ex)
            {
                return $"[json.encode ERROR] {ex.Message}";
            }
        }

        public DynValue decode(Script script, string json)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject(json);
                return JsonTableConverter.ToLuaTable(script, obj);
            }
            catch (Exception ex)
            {
                return DynValue.NewString($"[json.decode ERROR] {ex.Message}");
            }
        }

        private object ConvertDynValueToObject(DynValue val)
        {
            if (val == null) return null;

            switch (val.Type)
            {
                case DataType.String: return val.String;
                case DataType.Number: return val.Number;
                case DataType.Boolean: return val.Boolean;
                case DataType.Table: return ConvertTable(val.Table);
                default: return $"[Unsupported Type: {val.Type}]";
            }
        }

        private object ConvertTable(Table table)
        {
            // Check if array-like
            bool isArray = IsArrayTable(table);

            if (isArray)
            {
                var list = new List<object>();
                foreach (var pair in table.Values)
                {
                    list.Add(ConvertDynValueToObject(pair));
                }
                return list;
            }
            else
            {
                var dict = new Dictionary<string, object>();
                foreach (var pair in table.Pairs)
                {
                    string key = pair.Key.ToPrintString();
                    dict[key] = ConvertDynValueToObject(pair.Value);
                }
                return dict;
            }
        }

        private bool IsArrayTable(Table table)
        {
            int count = 0;
            foreach (var pair in table.Pairs)
            {
                if (pair.Key.Type != DataType.Number)
                    return false;
                count++;
            }
            return count > 0;
        }
    }
}
