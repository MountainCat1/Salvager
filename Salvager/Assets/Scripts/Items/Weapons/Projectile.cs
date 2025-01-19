using System;
using Managers;
using Markers;
using UnityEngine;
using Utilities;
using Zenject;

namespace Items.Weapons
{
    public class Projectile : MonoBehaviour
    {
        public event Action<Creature, AttackContext> Hit;

        [Inject] private ISoundPlayer _soundPlayer;

        [SerializeField] private ColliderEventProducer colliderEventProducer;
        [SerializeField] private AudioClip hitSound;

        public float Speed { get; private set; }
        public float Damage { get; private set; }

        private bool _isLaunched = false;
        private bool _initialized = false;
        private AttackContext _attackContext;

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
            if (CollisionUtility.IsObstacle(other.gameObject))
            {
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

            if(hitCreature.GetAttitudeTowards(_attackContext.Attacker) == Attitude.Friendly)
                return;
            
            try
            {
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
    }
}