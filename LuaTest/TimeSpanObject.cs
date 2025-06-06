using System;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class TimeSpanObject
{
    private readonly TimeSpan _ts;

    public TimeSpanObject(TimeSpan ts)
    {
        _ts = ts;
    }

    public double TotalSeconds => _ts.TotalSeconds;
    public double TotalMilliseconds => _ts.TotalMilliseconds;
}
