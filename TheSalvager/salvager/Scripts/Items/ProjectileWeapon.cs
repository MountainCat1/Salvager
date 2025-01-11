using System;
using Godot;
using Services;

namespace Items;

public partial class ProjectileWeapon : Weapon
{
    [Inject] private ISpawnerManager _spawnerManager = null!;

    [Export] private PackedScene _projectilePrefab = null!;
    [Export] private float _projectileSpeed = 400f;

    [Export] public AudioStream? AttackSound { get; set; }
    
    protected override void Attack(AttackContext ctx)
    {
        var direction = ctx.Direction;

        var projectile = _spawnerManager.SpawnCreature<Projectile>(
            _projectilePrefab,
            ctx.Attacker.Position,
            0,
            null
        );

        if (projectile == null)
        {
            throw new NullReferenceException("Failed to spawn projectile");
        }

        Vector2 normalizedDirection = direction.Normalized();
        float angle = Mathf.Atan2(normalizedDirection.Y, normalizedDirection.X) * (180 / Mathf.Pi);
        projectile.RotationDegrees = angle;
        

        projectile.Initialize(
            speed: _projectileSpeed,
            damage: CalculateDamage(ctx)
        );

        projectile.Hit += OnProjectileHit;

        projectile.Launch(ctx);
        
        if(AttackSound != null)
            SoundPlayer.PlaySound(AttackSound, ctx.Attacker.Position);
    }

    private void OnProjectileHit(Creature? hitCreature, AttackContext attackCtx)
    {
        if (hitCreature == null)
        {
            return;
        }
        
        var hitCtx = new HitContext()
        {
            Attacker = attackCtx.Attacker,
            Damage = CalculateDamage(attackCtx),
            Target = hitCreature,
            PushFactor = PushFactor
        };

        OnHit(hitCreature, hitCtx);
    }
}