using System;
using Godot;

public partial class Creature : Entity
{
    public event Action<HitContext> Hit;
    
    
    [Export] private float Speed = 300f;
    [Export] private float Accel = 7f;
    
    public float SightRange { get; set; }
    public NavigationAgent2D NavigationAgent => _nav;
    public CreatureController Controller { get; set; }
    public IReadonlyRangedValue Health { get; }

    private NavigationAgent2D _nav;
    private Vector2 _velocity = Vector2.Zero;

    public override void _Ready()
    {
        _nav = GetNode<NavigationAgent2D>("NavigationAgent2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        // If the character is close to the target position, don't move
        if(Position.DistanceTo(_nav.TargetPosition) < 10)
            return;

        // Calculate the next direction based on the NavigationAgent2D's pathfinding
        Vector2 direction = (_nav.GetNextPathPosition() - GlobalPosition).Normalized();

        // Smoothly adjust the velocity towards the target
        _velocity = _velocity.Lerp(direction * Speed, Accel * (float)delta);

        Velocity = _velocity;
    }

    public void Damage(HitContext hitContext)
    {
        // warining
        GD.PushWarning("Creature.Damage() is not implemented");
    }

    public void StartUsingWeapon(Weapon weapon)
    {
        GD.PushWarning("Creature.StartUsingWeapon(Weapon weapon) is not implemented");
    }
}