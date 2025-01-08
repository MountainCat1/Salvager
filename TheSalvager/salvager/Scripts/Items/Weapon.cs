using System;
using Godot;
using Services;

namespace Items;

public partial class Weapon : Item
{
    [Inject] protected ISoundPlayer _soundPlayer = null!;
    
    [Export] public float Range { get; set; }

    [Export] public float BaseAttackSpeed { get; set; }

    [Export] public float BaseDamage { get; set; }
    [Export] public float PushFactor { get; set; }
    [Export] public AudioStream HitSound { get; set; }

    public virtual bool AllowToMoveOnCooldown => false;
    public virtual bool NeedsLineOfSight => false;
    public bool IsOnCooldown => GetOnCooldown(new AttackContext());

    private DateTime _lastAttackTime;

    public bool GetOnCooldown(AttackContext ctx)
    {
        return DateTime.Now - _lastAttackTime < TimeSpan.FromSeconds(1f / CalculateAttackSpeed(ctx));
    }

    public void PerformAttack(AttackContext ctx)
    {
        _lastAttackTime = DateTime.Now;

        Attack(ctx);
    }

    public void ContinuousAttack(AttackContext ctx)
    {
        if (GetOnCooldown(ctx))
            return;

        PerformAttack(ctx);
    }

    protected virtual void Attack(AttackContext ctx) {}

    protected virtual void OnHit(Creature target, HitContext hitContext)
    {
        if (target != null)
        {
            target.Damage(hitContext);
            _soundPlayer.PlaySound(HitSound, Position);
        }
    }

    protected Vector2 CalculatePushForce(Creature target)
    {
        var direction = (target.Position - Position).Normalized();
        // var pushForce = direction * (PushFactor * (BaseDamage / target.Health.MaxValue));
        // return pushForce;

        return direction;
    }

    public override void Use(ItemUseContext ctx)
    {
        base.Use(ctx);

        ctx.Creature.StartUsingWeapon(this);
    }

    private float CalculateAttackSpeed(AttackContext ctx)
    {
        return BaseAttackSpeed;
    }

    public float CalculateDamage(AttackContext ctx)
    {
        return BaseDamage;
    }
}


public struct AttackContext
{
    public Vector2 Direction { get; set; }
    public Creature Target { get; set; }
    public Creature Attacker { get; set; }
}

public struct HitContext
{
    public HitContext(Creature attacker, Creature target, float damage, float pushFactor = 1)
    {
        Attacker = attacker;
        Target = target;
        Damage = damage;
        PushFactor = pushFactor;
    }

    public Creature Attacker { get; set; }
    public Creature Target { get; set; }
    public float Damage { get; set; }
    public float PushFactor { get; set; }
    public Vector2 Push => GetPushForce();

    public void ValidateAndLog()
    {
        if (Attacker == null)
            GD.PrintErr("Attacker is null");

        if (Target == null)
            GD.PrintErr("Target is null");

        if (Damage <= 0)
            GD.PrintErr("Damage is less than or equal to 0");

        if (Push == Vector2.Zero)
            GD.PrintErr("PushForce is zero");
    }

    public Vector2 GetPushForce()
    {
        var direction = (Target.Position - Attacker.Position).Normalized();
        var pushForce = direction * (PushFactor * (Damage / Target.Health.MaxValue));
        return pushForce;
    }
}