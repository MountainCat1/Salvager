using System;
using Components;
using Managers;
using Markers;
using UnityEngine;
using Utilities;
using Zenject;

namespace Items.Weapons
{
    public class ProjectileSettings
    {
        public float BaseMissChance = 0.05f;
        public float MissChanceForObstacle = 0.6f;
        public float MissChanceForFriendly = 0.95f;
    }

    public abstract class Projectile : MonoBehaviour
    {
        private const float Lifetime = 16f;

        public event Action<IDamageable, AttackContext> Hit;
        public event Action<AttackContext, Entity> Missed;

        [Inject] protected ISoundPlayer SoundPlayer;

        [SerializeField] private AudioClip hitSound;

        public float Speed { get; protected set; }
        public float Damage { get; protected set; }

        protected bool _isLaunched = false;
        protected bool _initialized = false;
        protected AttackContext _attackContext;
        protected ProjectileSettings _settings = new ProjectileSettings();

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
            Destroy(gameObject, Lifetime);
        }

        public virtual void Initialize(float speed, float damage)
        {
            Speed = speed;
            Damage = damage;

            _initialized = true;
        }

        public virtual void Launch(AttackContext ctx)
        {
            if (!_initialized) throw new Exception("Projectile not initialized");

            _attackContext = ctx;

            _isLaunched = true;
        }

        protected virtual void OnProjectileCollision(Collider2D other)
        {
            if (CollisionUtility.IsWall(other.gameObject))
            {
                HandleWallCollision();
                return;
            }

            if (CollisionUtility.IsObstacle(other.gameObject))
            {
                if (TryMiss(_settings.MissChanceForObstacle, other.GetComponent<Entity>()))
                    return;

                HandleHit();
                return;
            }

            var damageableCollider = other.GetComponent<DamageableCollider>();
            
            if (Creature.IsCreature(other.gameObject) == false)
            {
                if(damageableCollider.Damagable != null)
                {
                    HandleHit(damageableCollider.Damagable);
                }

                return;
            };
            
            var creatureCollider = other.GetComponent<CreatureCollider>(); 
            
            var hitCreature = creatureCollider.Creature;

            if (hitCreature == null)
            {
                Debug.LogError("Projectile hit something that is not a creature");
                return;
            }

            if (hitCreature == _attackContext.Attacker) return;

            if (hitCreature.GetAttitudeTowards(_attackContext.Attacker) ==
                Attitude.Friendly)
            {
                if (TryMiss(_settings.MissChanceForFriendly, hitCreature)) return;
            }

            if (TryMiss(_settings.BaseMissChance, hitCreature)) return;

            HandleHit(hitCreature);
        }

        protected virtual void HandleWallCollision()
        {
            PlayHitSound();
            Hit?.Invoke(null, _attackContext);
            Destroy(gameObject);
        }

        protected virtual void HandleHit()
        {
            PlayHitSound();
            Hit?.Invoke(null, _attackContext);
            Destroy(gameObject);
        }

        protected virtual void HandleHit(IDamageable damageable)
        {
            try
            {
                PlayHitSound();
                Hit?.Invoke(damageable, _attackContext);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            Destroy(gameObject);
        }

        protected virtual void PlayHitSound()
        {
            if (hitSound) SoundPlayer.PlaySound(hitSound, transform.position, SoundType.Sfx);
        }

        protected virtual void Update()
        {
        }

        protected bool TryMiss(float baseChance, Entity entity)
        {
            if (UnityEngine.Random.value < baseChance)
            {
                Missed?.Invoke(_attackContext, entity);
                Debug.Log("Projectile missed");
                return true;
            }

            return false;
        }
    }
}