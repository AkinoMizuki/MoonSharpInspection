using System;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class DateTimeAPI
{
    public static DateTimeObject Now()
    {
        return new DateTimeObject(DateTime.Now);
    }

    public static DateTimeObject UtcNow()
    {
        return new DateTimeObject(DateTime.UtcNow);
    }

    public static DateTimeObject FromUnixTime(double unixSeconds)
    {
        var dt = DateTimeOffset.FromUnixTimeSeconds((long)unixSeconds).DateTime;
        return new DateTimeObject(dt);
    }

    public static DateTimeObject Parse(string dateTimeStr)
    {
        DateTime dt = DateTime.Parse(dateTimeStr);
        return new DateTimeObject(dt);
    }

    public static DateTimeObject New(int year, int month, int day)
    {
        return new DateTimeObject(new DateTime(year, month, day));
    }

    public static DateTimeObject NewFull(int year, int month, int day, int hour, int minute, int second)
    {
        return new DateTimeObject(new DateTime(year, month, day, hour, minute, second));
    }
}
