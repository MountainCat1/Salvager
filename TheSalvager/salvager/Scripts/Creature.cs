using System;
using Godot;
using Items;
using Services;

public partial class Creature : Entity
{
    public event Action<HitContext>? Hit;
    
    [Export] private float _speed = 300f;
    [Export] private float _accel = 7f;
    
    public float SightRange { get; set; } = 1500f;
    public NavigationAgent2D NavigationAgent => _nav;
    public CreatureControllers.CreatureController Controller { get; set; } = null!;
    public IReadonlyRangedValue Health => _health;
    [Export] public Weapon? Weapon { get; private set; } 
    [Export] private float MaxHealth { get; set; }
    [Export] public Teams Team { get; private set; }
    
    private RangedValue _health = null!;
    private NavigationAgent2D _nav = null!;
    private Vector2 _velocity = Vector2.Zero;

    public override void _Ready()
    {
        _health = new RangedValue(MaxHealth, 0, MaxHealth);
        _health.ValueChanged += OnHealthChanged;
        
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
        _velocity = _velocity.Lerp(direction * _speed, _accel * (float)delta);

        Velocity = _velocity;
    }

    public void Damage(HitContext hitContext)
    {
        GD.Print($"Creature got hit for {hitContext.Damage} damage by {hitContext.Attacker.Name}");
        
        _health.CurrentValue -= hitContext.Damage;
        
        Hit?.Invoke(hitContext);
    }

    public void StartUsingWeapon(Items.Weapon weapon)
    {
        GD.PushWarning("Creature.StartUsingWeapon(Weapon weapon) is not implemented");
    }
    
    private void OnHealthChanged()
    {
        if (_health.CurrentValue <= 0)
        {
            QueueFree();
        }
    }

}