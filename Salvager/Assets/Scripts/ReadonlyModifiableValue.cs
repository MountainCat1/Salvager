using System;

public interface IReadonlyRangedValue 
{
    public float BaseValue { get; }

    public float CurrentValue{ get; }
    public float MinValue{ get; }
    
    public float MaxValue { get; }
    
    public event Action ValueChanged;
}