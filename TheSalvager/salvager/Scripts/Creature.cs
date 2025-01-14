using System;
using Godot;
using Items;
using Microsoft.VisualBasic;
using Services;

public partial class Creature : Entity
{
    public event Action<HitContext>? Hit;
    public event Action<CreatureInteraction>? Interacted;

    [Export] private float _speed = 300f;
    [Export] private float _accel = 7f;

    public float SightRange { get; set; } = 1500f;
    public NavigationAgent2D NavigationAgent => _nav;
    public CreatureControllers.CreatureController Controller { get; set; } = null!;
    public IReadonlyRangedValue Health => _health;
    [Export] public Weapon? Weapon { get; private set; }
    [Export] private float MaxHealth { get; set; }
    [Export] public Teams Team { get; private set; }
    public float InteractionRange { get; set; } = 15f;

    private RangedValue _health = null!;
    private NavigationAgent2D _nav = null!;
    private Vector2 _velocity = Vector2.Zero;

    public override void _Ready()
    {
        _health = new RangedValue(MaxHealth, 0, MaxHealth);
        _health.ValueChanged += OnHealthChanged;

        _nav = GetNode<NavigationAgent2D>("NavigationAgent2D");
        _nav.TargetPosition = GlobalPosition;

        _nav.VelocityComputed += (velocity) => { _velocity = velocity; };
    }

    public override void _PhysicsProcess(double delta)
    {
        if (GlobalPosition.DistanceTo(_nav.TargetPosition) < 10)
        {
            _velocity = Vector2.Zero;
            _nav.SetVelocity(Vector2.Zero);
            return;
        }

        Vector2 direction = (_nav.GetNextPathPosition() - GlobalPosition).Normalized();

        var newVelocity = _velocity.Lerp(direction * _speed, _accel * (float)delta);

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