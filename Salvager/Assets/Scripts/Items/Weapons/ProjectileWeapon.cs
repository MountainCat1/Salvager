﻿using Components;
using Managers;
using UnityEngine;
using Zenject;

namespace Items.Weapons
{
    public class ProjectileWeapon : Weapon
    {
        [Inject] private DiContainer _diContainer;
        [Inject] private IProjectileManager _projectileManager;

        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private float projectileSpeed;

        public override bool NeedsLineOfSight => true;

        protected override void Attack(AttackContext ctx)
        {
            var direction = ctx.Direction;

            var projectile = _projectileManager.SpawnProjectile(projectilePrefab, transform.position);
            
            Vector2 normalizedDirection = direction.normalized;
            float angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

            var damage = WeaponItemData.GetApplied(WeaponPropertyModifiers.Damage, BaseDamage);

            projectile.Initialize(
                speed: projectileSpeed,
                damage: CalculateDamage(damage, ctx)
            );

            projectile.Hit += OnProjectileHit;

            projectile.Launch(ctx);
        }

        

        private void OnProjectileHit(IDamageable damageable, AttackContext attackCtx)
        {
            var damage = WeaponItemData.GetApplied(WeaponPropertyModifiers.Damage, BaseDamage);
            
            var hitCtx = new HitContext()
            {
                Attacker = attackCtx.Attacker,
                Damage = CalculateDamage(damage, attackCtx),
                Target = damageable,
                PushFactor = PushFactor
            };

            OnHit(damageable, hitCtx);
        }
    }
}