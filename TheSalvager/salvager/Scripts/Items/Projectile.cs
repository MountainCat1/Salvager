using System;
using Godot;
using Services;
using Utilities;

namespace Items;

public partial class Projectile : Area2D
{
    public event Action<Creature?, AttackContext>? Hit;

    [Inject] private ISoundPlayer _soundPlayer = null!;

    [Export] public float Speed { get; private set; }
    [Export] public float Damage { get; private set; }
    [Export] public AudioStream? HitSound;


    private bool _isLaunched = false;
    private bool _initialized = false;
    private AttackContext _attackContext;

    public override void _Ready()
    {
    }
    
    public void Initialize(float speed, float damage)
    {
        Speed = speed;
        Damage = damage;

        _initialized = true;
    }

    public void Launch(AttackContext ctx)
    {
        if (!_initialized)
            throw new Exception("Projectile not initialized");

        _attackContext = ctx;

        BodyEntered += OnBodyEntered;

        _isLaunched = true;
    }

    private void OnBodyEntered(Node body)
    {
        if (CollisionUtility.IsObstacle(body))
        {
            GD.Print($"Projectile hit obstacle {body.Name}");
            Hit?.Invoke(null, _attackContext);
            QueueFree();
            return;
        }

        var creature = body as Creature;
        
        if (creature == null)
        {
            GD.PrintErr("Projectile hit something that is not a creature - " + body.Name);
            return;
        }

        if (creature == _attackContext.Attacker)
            return;

        try
        {
            if (HitSound != null)
                _soundPlayer.PlaySound(HitSound, Position, SoundType.Sfx);

            Hit?.Invoke(creature, _attackContext);
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
        }

        QueueFree();
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (!_isLaunched)
            return;
        
        Position += Transform.X * (float)(delta * Speed); 
    }
}