using System;
using Items;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public abstract class Weapon : ItemBehaviour
{
    [Inject] private ISoundPlayer _soundPlayer;

    [field: SerializeField] public float Range { get; set; }

    [field: SerializeField]
    [field: FormerlySerializedAs("AttackSpeed")]
    public float BaseAttackSpeed { get; set; }

    [field: SerializeField] public float BaseDamage { get; set; }
    [field: SerializeField] public float PushFactor { get; set; }
    [field: SerializeField] public AudioClip HitSound { get; set; }

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

    public void ContiniousAttack(AttackContext ctx)
    {
        if (GetOnCooldown(ctx))
            return;

        PerformAttack(ctx);
    }

    protected abstract void Attack(AttackContext ctx);

    protected virtual void OnHit(Creature target, HitContext hitContext)
    {
        if (target != null)
        {
            target.Damage(hitContext);
        }

        if (HitSound != null)
            _soundPlayer.PlaySound(HitSound, transform.position);
    }

    protected Vector2 CalculatePushForce(Creature target)
    {
        var direction = (target.transform.position - transform.position).normalized;
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
        return BaseAttackSpeed * (1 + ctx.Attacker.LevelSystem.CharacteristicsLevels[Characteristics.Dexterity] * CharacteristicsConsts.AttackSpeedAdditiveMultiplierPerDexterity);
    }
    
    public float CalculateDamage(AttackContext ctx)
    {
        return BaseDamage + ctx.Attacker.LevelSystem.CharacteristicsLevels[Characteristics.Strength] * CharacteristicsConsts.DamageAdditiveMultiplierPerStrength;
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
            Debug.LogError("Attacker is null");

        if (Target == null)
            Debug.LogError("Target is null");

        if (Damage <= 0)
            Debug.LogError("Damage is less than or equal to 0");

        if (Push == Vector2.zero)
            Debug.LogError("PushForce is zero");
    }

    public Vector2 GetPushForce()
    {
        var direction = (Target.transform.position - Attacker.transform.position).normalized;
        var pushForce = direction * (PushFactor * (Damage / Target.Health.MaxValue));
        return pushForce;
    }
}