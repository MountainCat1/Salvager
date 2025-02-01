using System;
using Items;
using Managers;
using Managers.Visual;
using UnityEngine;
using Zenject;

public abstract class Weapon : ItemBehaviour
{
    public event Action<AttackContext> Attacked;
    
    [Inject] private ISoundPlayer _soundPlayer;
    [Inject] private ICameraShakeService _cameraShakeService;

    [field: SerializeField] public float Range { get; set; }

    [field: SerializeField] public float BaseAttackSpeed { get; set; }

    [field: SerializeField] public float BaseDamage { get; set; }
    [field: SerializeField] public float PushFactor { get; set; }
    [field: SerializeField] public float ShakeFactor { get; set; }
    [field: SerializeField] public AudioClip HitSound { get; set; }
    [field: SerializeField] public AudioClip AttackSound { get; set; }

    public virtual bool AllowToMoveOnCooldown => false;
    public virtual bool NeedsLineOfSight => false;
    public bool IsOnCooldown => GetOnCooldown(new AttackContext());

    private float _lastAttackTime = -1;
    
    protected const float RandomPitch = 0.3f;

    public bool GetOnCooldown(AttackContext ctx)
    {
        // TODO: DO NOT USE DateTime.Now in Update or FixedUpdate, it is very slow coz it is a system call
        return Time.time - _lastAttackTime < 1f / CalculateAttackSpeed(ctx);
    }

    public void PerformAttack(AttackContext ctx)
    {
        _lastAttackTime = Time.time;

        _cameraShakeService.ShakeCamera(ctx.Attacker.transform.position, ShakeFactor);
        
        if(AttackSound != null)
            _soundPlayer.PlaySound(AttackSound, transform.position);
        
        Attacked?.Invoke(ctx);
        Attack(ctx);
    }

    public void ContinuousAttack(AttackContext ctx)
    {
        if (GetOnCooldown(ctx))
            return;

        PerformAttack(ctx);
    }

    // !!!
    // Should always ONLY be invoked by PerformAttack, to ensure that the weapon is on cooldown and events are invoked
    // !!!
    protected abstract void Attack(AttackContext ctx);

    protected virtual void OnHit(Creature target, HitContext hitContext)
    {
        if (target != null)
        {
            target.Damage(hitContext);
        }

        if (HitSound != null)
            _soundPlayer.PlaySound(HitSound, transform.position, SoundType.Sfx, RandomPitch);
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
        if (ctx.Attacker == null)
            return BaseAttackSpeed;


        var dexterityModifier = ctx.Attacker.LevelSystem.CharacteristicsLevels[Characteristics.Dexterity] *
                                CharacteristicsConsts.AttackSpeedAdditiveMultiplierPerDexterity;

        return BaseAttackSpeed * (1 + dexterityModifier);
    }

    public float CalculateDamage(AttackContext ctx)
    {
        return BaseDamage + ctx.Attacker.LevelSystem.CharacteristicsLevels[Characteristics.Strength] *
            CharacteristicsConsts.DamageAdditiveMultiplierPerStrength;
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
        // If the attacker is dead then the attacker is null, so we can't check for null
        // TODO: Save dead creatures instead of removing them from memory
        // if (Attacker == null)
        //     Debug.LogError("Attacker is null");

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