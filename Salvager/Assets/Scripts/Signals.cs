using System;

public enum Signals
{
    CristalDestroyed,
    Win,
    GameOver,
    CloseGame
}

public static class SignalExtensions
{
    public static string ToSignal(this Signals signal)
    {
        return signal.ToString();
    }
    
    public static Signals ToSignal(this string signal)
    {
        return (Signals) Enum.Parse(typeof(Signals), signal);
    }
}