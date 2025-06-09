using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;

public static class JsonTableConverter
{
    public static DynValue ToLuaTable(Script script, object jsonObj)
    {
        if (jsonObj is JObject jObj)
        {
            var table = new Table(script);
            foreach (var prop in jObj.Properties())
            {
                table[prop.Name] = ToLuaTable(script, prop.Value);
            }
            return DynValue.NewTable(table);
        }
        else if (jsonObj is JArray jArray)
        {
            var table = new Table(script);
            int i = 1;
            foreach (var item in jArray)
            {
                table[i++] = ToLuaTable(script, item);
            }
            return DynValue.NewTable(table);
        }
        else if (jsonObj is JValue jVal)
        {
            return DynValue.FromObject(script, jVal.Value);
        }
        else if (jsonObj is IDictionary dict)
        {
            var table = new Table(script);
            foreach (DictionaryEntry kv in dict)
            {
                table[kv.Key.ToString()] = ToLuaTable(script, kv.Value);
            }
            return DynValue.NewTable(table);
        }
        else if (jsonObj is IEnumerable enumerable)
        {
            var table = new Table(script);
            int i = 1;
            foreach (var item in enumerable)
            {
                table[i++] = ToLuaTable(script, item);
            }
            return DynValue.NewTable(table);
        }

        return DynValue.FromObject(script, jsonObj);
    }
}
