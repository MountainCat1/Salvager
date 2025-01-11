
using System;
using Godot;

public partial class FlashlightAnimation : PointLight2D
{
    private Light2D _light2D;
    [Export] private float _flickeringSpeed = 0.1f;
    [Export] private float _flickerMax = 0.1f;
    [Export] private float _offsetSpeed = 0.1f;
    [Export] private float _offsetMax = 0.1f;

    private float _baseEnergy;
    private Vector2 _baseOffset;
    
    public override void _Ready()
    {
        base._Ready();
        
        _baseEnergy = Energy;
        _baseOffset = Offset;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        // use datetime
        var tick = DateTime.Now.Ticks / 1000000000.0;
        
        Energy = _baseEnergy + (float) Math.Sin(tick * _flickeringSpeed) * _flickerMax;
        Offset = _baseOffset + new Vector2((float) Math.Sin(tick * _offsetSpeed) * _offsetMax, 0);
    }
}