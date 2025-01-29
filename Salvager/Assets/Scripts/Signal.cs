using System;

public enum Signal
{
    TerminalHacked
}

public static class SignalExtensions
{
    public static string ToSignal(this Signal signal)
    {
        return signal.ToString();
    }
    
    public static Signal ToSignal(this string signal)
    {
        return (Signal) Enum.Parse(typeof(Signal), signal);
    }
}