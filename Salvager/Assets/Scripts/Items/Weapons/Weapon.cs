using System;
using System.Collections.Generic;
using System.Linq;
using Components;
using Data;
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

    // [field: SerializeField] public float Range { get; set; }
    // [field: SerializeField] public float BaseAttackSpeed { get; set; }
    // [field: SerializeField] public float BaseDamage { get; set; }

    [SerializeField] private WeaponData weaponData;

    public override ItemData ItemData
    {
        get => weaponData;
        protected set => weaponData = value as WeaponData ?? throw new ArgumentException($"{value.GetType().FullName} is not WeaponData");
    }
    
    [field: SerializeField] public float PushFactor { get; set; }
    [field: SerializeField] public float ShakeFactor { get; set; }
    [field: SerializeField] public AudioClip HitSound { get; set; }
    [field: SerializeField] public AudioClip AttackSound { get; set; }

    public float AttackSpeed => weaponData.BaseAttackSpeed;
    public float Damage => weaponData.GetDamage();
    public float Range => weaponData.Range;
    
    public virtual bool AllowToMoveOnCooldown => false;
    public virtual bool NeedsLineOfSight => false;
    public virtual bool ShootThroughAllies => false;
    public override bool Stackable => false;

    public bool IsOnCooldown => GetOnCooldown(new AttackContext());
    
    private float _lastAttackTime = -1;
    private List<WeaponValueModifier> _modifiers = new();

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

        if (AttackSound != null)
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

    protected virtual void OnHit(IDamageable damageable, HitContext hitContext)
    {
        if (damageable != null)
        {
            damageable.Health.Damage(hitContext);
        }

        if (HitSound != null)
            _soundPlayer.PlaySound(HitSound, transform.position, SoundType.Sfx, RandomPitch);
    }

    protected Vector2 CalculatePushForce(Creature target)
    {
        var direction = (target.transform.position - transform.position).normalized;
        // var pushForce = direction * (PushFactor * (_baseDamage / target.Health.MaxValue));
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
            return weaponData.BaseAttackSpeed;


        var dexterityModifier = ctx.Attacker.LevelSystem.CharacteristicsLevels[Characteristics.Dexterity] *
                                CharacteristicsConsts.AttackSpeedAdditiveMultiplierPerDexterity;

        return AttackSpeed * (1 + dexterityModifier);
    }
    
    private float GetDamage()
    {
        return CalculateDamage(Damage, _modifiers);
    }

    public static float CalculateDamage(float damage, AttackContext ctx)
    {
        return damage + ctx.Attacker.LevelSystem.CharacteristicsLevels[Characteristics.Strength] *
            CharacteristicsConsts.DamageAdditiveMultiplierPerStrength;
    }
    
    public static float CalculateDamage(float baseDamage, List<WeaponValueModifier> modifiers)
    {
        var damage = baseDamage + modifiers
            .Where(x => x.Name == WeaponPropertyModifiers.Damage)
            .Sum(x => x.Value);

        return damage;
    }

    public override void SetData(ItemData itemData)
    {
        base.SetData(itemData);

        _modifiers = itemData.Modifiers.OfType<WeaponValueModifier>().ToList();
        
        weaponData = itemData as WeaponData ?? throw new ArgumentException($"{itemData.GetType().FullName} is not WeaponData");
    }
}


public struct AttackContext
{
    public Vector2 Direction { get; set; }
    public Vector2 TargetPosition { get; set; }
    public Creature Target { get; set; }
    public Creature Attacker { get; set; }
}

public struct HitContext
{
    public HitContext(Creature attacker, IDamageable target, float damage, float pushFactor = 1)
    {
        Attacker = attacker;
        Target = target;
        Damage = damage;
        PushFactor = pushFactor;
    }

    public Creature Attacker { get; set; }
    public IDamageable Target { get; set; }
    public float Damage { get; set; }
    public float PushFactor { get; set; }
    public Vector2 Push => GetPushForce();

    public Vector3 Direction => Target is null || Attacker is null
            ? Vector3.zero
            : (Target.Health.transform.position - Attacker.transform.position).normalized;

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
    }

    public Vector2 GetPushForce()
    {
        var direction = (Target.Health.transform.position - Attacker.transform.position).normalized;
        var pushForce = direction * (PushFactor * (Damage / Target.Health.MaxValue));
        return pushForce;
    }
}