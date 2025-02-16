namespace Items.Weapons
{
    using UnityEngine;

    namespace Items.Weapons
    {
        public class ForwardProjectile : Projectile
        {
            [SerializeField] private ColliderEventProducer colliderEventProducer;

            public override void Launch(AttackContext ctx)
            {
                base.Launch(ctx);
                
                colliderEventProducer.TriggerEnter += OnProjectileCollision;
            }

            protected override void Update()
            {
                if (!_isLaunched) return;

                transform.position +=
                    (Vector3)(_attackContext.Direction * (Speed * Time.deltaTime));
            }
        }
    }
}