using System;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class DateTimeObject
{
    private readonly DateTime _dt;

    public DateTimeObject(DateTime dt)
    {
        _dt = dt;
    }

    // プロパティ（Luaから .Year などでアクセス可能）
    public int Year => _dt.Year;
    public int Month => _dt.Month;
    public int Day => _dt.Day;
    public int Hour => _dt.Hour;
    public int Minute => _dt.Minute;
    public int Second => _dt.Second;

    // Unix秒（1970年からの経過秒数）
    public double ToUnixTime()
    {
        return (new DateTimeOffset(_dt)).ToUnixTimeSeconds();
    }

    // C形式フォーマットを.NET形式に変換するヘルパー
    private string ConvertFormat(string format)
    {
        return format
            .Replace("%Y", "yyyy")
            .Replace("%y", "yy")
            .Replace("%m", "MM")
            .Replace("%d", "dd")
            .Replace("%H", "HH")
            .Replace("%I", "hh")
            .Replace("%M", "mm")
            .Replace("%S", "ss")
            .Replace("%p", "tt"); // AM/PM
    }

    // 日付をフォーマット付きで文字列に変換
    public string ToString(string format = null)
    {
        try
        {
            if (string.IsNullOrEmpty(format))
                return _dt.ToString("yyyy-MM-dd HH:mm:ss");

            string netFormat = ConvertFormat(format);
            return _dt.ToString(netFormat);
        }
        catch (FormatException)
        {
            return "[Invalid Format]";
        }
    }

    // 秒を加算して新しい DateTimeObject を返す
    public DateTimeObject AddSeconds(double sec)
    {
        return new DateTimeObject(_dt.AddSeconds(sec));
    }

    // 日数を加算して新しい DateTimeObject を返す
    public DateTimeObject AddDays(double days)
    {
        return new DateTimeObject(_dt.AddDays(days));
    }

    // 他の日付との差を TimeSpanObject として返す
    public TimeSpanObject Subtract(DateTimeObject other)
    {
        return new TimeSpanObject(_dt - other._dt);
    }

    // 他の日時との大小比較
    public int CompareTo(DateTimeObject other)
    {
        return _dt.CompareTo(other._dt);
    }

    // MoonSharp デフォルトで表示される ToString()
    [MoonSharpHidden]
    public override string ToString()
    {
        return ToString("yyyy-MM-dd HH:mm:ss");
    }

    // 元の DateTime を取得したい場合のプロパティ（必要に応じて）
    public DateTime RawValue => _dt;
}
