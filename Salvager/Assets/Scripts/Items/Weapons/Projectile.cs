using System;
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
        public float MissChanceForObstacle = 0.5f;
        public float MissChanceForFriendly = 0.5f;
    }
    
    public class Projectile : MonoBehaviour
    {
        private const float Lifetime = 16f;
        
        public event Action<Creature, AttackContext> Hit;
        public event Action<AttackContext, Entity> Missed;

        [Inject] private ISoundPlayer _soundPlayer;

        [SerializeField] private ColliderEventProducer colliderEventProducer;
        [SerializeField] private AudioClip hitSound;

        public float Speed { get; private set; }
        public float Damage { get; private set; }


        private bool _isLaunched = false;
        private bool _initialized = false;
        private AttackContext _attackContext;
        private ProjectileSettings _settings = new ProjectileSettings();

        private void Start()
        {
            Destroy(gameObject, Lifetime);
        }

        public void Initialize(float speed, float damage)
        {
            Speed = speed;
            Damage = damage;

            _initialized = true;
        }


        public void Launch(AttackContext ctx)
        {
            if (!_initialized)
                throw new Exception("Projectile not initialized");

            _attackContext = ctx;

            colliderEventProducer.TriggerEnter += OnProjectileCollision;

            _isLaunched = true;
        }

        private void OnProjectileCollision(Collider2D other)
        {
            if(CollisionUtility.IsWall(other.gameObject))
            {
                if (hitSound)
                    _soundPlayer.PlaySound(hitSound, transform.position, SoundType.Sfx);
                
                Hit?.Invoke(null, _attackContext);
                Destroy(gameObject);
                return;
            }
            
            if (CollisionUtility.IsObstacle(other.gameObject))
            {
                if(TryMiss(_settings.MissChanceForObstacle, other.GetComponent<Entity>()))
                    return;
                
                if (hitSound)
                    _soundPlayer.PlaySound(hitSound, transform.position, SoundType.Sfx);
                
                Hit?.Invoke(null, _attackContext);
                Destroy(gameObject);
                return;
            }

            if (Creature.IsCreature(other.gameObject) == false)
                return;

            var hitCreature = other.GetComponent<CreatureCollider>()?.Creature;

            if (hitCreature == null)
            {
                Debug.LogError("Projectile hit something that is not a creature");
                return;
            }

            if (hitCreature == _attackContext.Attacker)
                return;

            if (hitCreature.GetAttitudeTowards(_attackContext.Attacker) == Attitude.Friendly)
            {
                if(TryMiss(_settings.MissChanceForFriendly, hitCreature))
                    return;
            }
            
            try
            {
                if(TryMiss(_settings.BaseMissChance, hitCreature))
                    return;
                
                if (hitSound)
                    _soundPlayer.PlaySound(hitSound, transform.position, SoundType.Sfx);
                
                Hit?.Invoke(hitCreature, _attackContext);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            Destroy(gameObject);
        }

        private void Update()
        {
            if (!_isLaunched)
                return;

            transform.position += (Vector3)(_attackContext.Direction * (Speed * Time.deltaTime));
        }

        private bool TryMiss(float baseChance, Entity entity)
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