using System;
using CreatureControllers;
using Godot;
using Items;
using Services;

public partial class Creature : Entity
{
    public event Action<HitContext>? Hit;
    public event Action<CreatureInteraction>? Interacted;

    [Export] private float _speed = 300f;
    [Export] private float _accel = 7f;

    public float SightRange { get; set; } = 1500f;
    public NavigationAgent2D NavigationAgent => _nav;
    public CreatureController Controller { get; set; } = null!;
    public IReadonlyRangedValue Health => _health;
    [Export] public Weapon? Weapon { get; private set; }
    [Export] private float MaxHealth { get; set; }
    [Export] public Teams Team { get; private set; }
    public float InteractionRange { get; set; } = 15f;

    private RangedValue _health = null!;
    private NavigationAgent2D _nav = null!;
    private Vector2 _velocity = Vector2.Zero;
    private Line2D _navPathLine = null!;
    
    public override void _Ready()
    {
        _health = new RangedValue(MaxHealth, 0, MaxHealth);
        _health.ValueChanged += OnHealthChanged;

        _nav = GetNode<NavigationAgent2D>("NavigationAgent2D");
        _nav.TargetPosition = GlobalPosition;

        _nav.VelocityComputed += (velocity) => { _velocity = velocity; };
        
        _navPathLine = GetNode<Line2D>("NavPathLine");
    }

    public override void _PhysicsProcess(double delta)
    {
        // 1. Stop if close to target
        if (GlobalPosition.DistanceTo(NavigationAgent.TargetPosition) < 10)
        {
            _velocity = Vector2.Zero;
            _nav.SetVelocity(Vector2.Zero);
            return;
        }

        // 2. Get path from the server
        var path = NavigationAgent.GetCurrentNavigationPath();

        _navPathLine.ClearPoints();
        
        // 4. Draw the path, if desired
        foreach (var point in path)
        {
            _navPathLine.AddPoint(point - GlobalPosition);
        }

        // 5. Compute velocity toward the second point
        var nextPathPosition = NavigationAgent.GetNextPathPosition();
        Vector2 direction = (nextPathPosition - GlobalPosition).Normalized();
        Vector2 newVelocity = _velocity.Lerp(direction * _speed, _accel * (float)delta);

        // 6. If using built-in avoidance, set velocity on your NavigationAgent2D
        if (_nav.AvoidanceEnabled)
        {
            _nav.SetVelocity(newVelocity);
        }
        else
        {
            _velocity = newVelocity;
        }

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

    public CreatureInteraction Interact(IInteractable interactionTarget, double timeDelta)
    {
        var interaction = interactionTarget.Interact(this, timeDelta);
        Interacted?.Invoke(interaction);
        return interaction;
    }
}