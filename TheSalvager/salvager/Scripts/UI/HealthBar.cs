using System.Diagnostics;
using Godot;

namespace UI;

public partial class HealthBar : ProgressBar
{
    [Export] private Creature _creature = null!;

    public void Start()
    {
        Debug.Assert(_creature != null, "Creature is null!");        
        
        OnHealthChanged();

        _creature.Health.ValueChanged += OnHealthChanged;
    }

    private void OnHealthChanged()
    {
        Value = _creature.Health.CurrentValue;
        MaxValue = _creature.Health.MaxValue;
        MinValue = _creature.Health.MinValue;
    }
}