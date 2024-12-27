using UnityEngine;
using Zenject;

namespace Items.Weapons
{
    public class ProjectileWeapon : Weapon
    {
        [Inject] private DiContainer _diContainer;

        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private float projectileSpeed;

        protected override void Attack(AttackContext ctx)
        {
            var direction = ctx.Direction;

            var projectileGo = _diContainer.InstantiatePrefab(
                projectilePrefab.gameObject,
                transform.position,
                Quaternion.identity,
                null
            );

            var projectile = projectileGo.GetComponent<Projectile>();

            Vector2 normalizedDirection = direction.normalized;
            float angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

            projectile.Initialize(
                speed: projectileSpeed,
                damage: CalculateDamage(ctx)
            );

            projectile.Hit += OnProjectileHit;

            projectile.Launch(ctx);
        }

        private void OnProjectileHit(Creature hitCreature, AttackContext attackCtx)
        {
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
}