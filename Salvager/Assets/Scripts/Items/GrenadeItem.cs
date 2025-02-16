using Items.Weapons;
using Managers;
using UnityEngine;
using Zenject;

namespace Items
{
    public class GrenadeItem : ActiveItemBehaviour
    {
        [Inject] private IProjectileManager _projectileManager;
        
        [SerializeField] private Projectile grenadePrefab;
        [SerializeField] private float speed;
        [SerializeField] private float damage;
        [SerializeField] private float pushFactor;

        public override bool Stackable => true;

        public override void Use(ItemUseContext ctx)
        {
            base.Use(ctx);
        }

        public override void UseActiveAbility(AbilityUseContext context)
        {
            base.UseActiveAbility(context);
            
            var grenade = _projectileManager.SpawnProjectile(grenadePrefab, context.User.transform.position);
            grenade.Initialize(speed, damage);

            var targetPosition = context.Target != null 
                ? (Vector2)context.Target.transform.position 
                : context.TargetPosition;
            
            var attackContext = new AttackContext()
            {
                Attacker = context.User,
                Target = context.Target,
                Direction = (targetPosition - (Vector2)context.User.transform.position).normalized,
                TargetPosition = targetPosition
            };
            
            grenade.Hit += (hitCreature, attackCtx) => OnProjectileHit(hitCreature, attackCtx);
            
            grenade.Launch(attackContext);
            
            Inventory.RemoveItems(GetIdentifier(), 1);
        }
        
        private void OnProjectileHit(Creature hitCreature, AttackContext attackCtx)
        {
            var hitCtx = new HitContext()
            {
                Attacker = attackCtx.Attacker,
                Damage = Weapon.CalculateDamage(damage, attackCtx),
                Target = hitCreature,
                PushFactor = pushFactor
            };

            hitCreature.Damage(hitCtx);
        }
    }
}