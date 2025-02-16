using System;
using Managers.Visual;
using Markers;
using UnityEngine;
using Zenject;

namespace Items.Weapons
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class GrenadeProjectile : Projectile
    { 
        [Inject] ICameraShakeService _cameraShakeService;
        
        [SerializeField] private float _initialAngle = 45f; // Launch angle in degrees
        [SerializeField] private float _fuseTime = 2.0f; // Time before explosion
        [SerializeField] private float radius = 3f; // Radius of the explosion
        [SerializeField] private float maxDistanceSpeedCorrection = 2f; // distance where the speed correction starts
        [SerializeField] private float correctionAmount = 5f; // distance where the speed correction starts

        [SerializeField] private GameObject _explosionPrefab; // Prefab for the explosion effect
        [SerializeField] private AudioClip _explosionSound; // Sound to play when the grenade explodes
        [SerializeField] private float cameraShakeFactor = 1; // Sound to play when the grenade explodes
        
        private Rigidbody2D _rb;
        private float _timeUntilExplosion;

        protected override void Awake()
        {
            base.Start();
            _rb = GetComponent<Rigidbody2D>();
            if (_rb == null)
            {
                Debug.LogError("GrenadeProjectile requires a Rigidbody2D component!");
                enabled = false; // Disable the script if no Rigidbody2D is found
            }
        }

        public override void Launch(AttackContext ctx)
        {
            base.Launch(ctx); // Call the base class's Launch method

            _timeUntilExplosion = _fuseTime;

            // Calculate initial velocity based on angle and speed
            float angleInRadians = _initialAngle * Mathf.Deg2Rad;
            Vector2 initialVelocity =
                new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
            initialVelocity = _attackContext.Direction * Speed;

            // Apply the initial velocity to the Rigidbody2D
            _rb.velocity = initialVelocity;
        }

        protected override void Update()
        {
            if (!_isLaunched) return;

            _timeUntilExplosion -= Time.deltaTime;

            if (_timeUntilExplosion <= 0)
            {
                Explode();
            }
        }

        private void FixedUpdate()
        {
            if(!_isLaunched)
                return;
            
            // Correct the speed so grenade will stop at the target
            var distance = Vector2.Distance(transform.position, _attackContext.TargetPosition);
            if (distance < maxDistanceSpeedCorrection)
            {
                _rb.velocity -= _rb.velocity.normalized * ((maxDistanceSpeedCorrection - distance) * Time.fixedDeltaTime * correctionAmount);
            }
        }

        private void Explode()
        {
            // Instantiate explosion prefab
            if (_explosionPrefab != null)
            {
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            }
            
            // Play explosion sound
            if (_explosionSound != null)
            {
                SoundPlayer.PlaySound(_explosionSound, transform.position);
            }
            
            _cameraShakeService.ShakeCamera(transform.position, Damage * cameraShakeFactor);

            // Apply damage to nearby creatures (example)
            Collider2D[] results = new Collider2D[50];
            var size = Physics2D.OverlapCircleNonAlloc(transform.position, radius, results);

            for (int i = 0; i < size; i++)
            {
                var hitCollider = results[i];

                if (Creature.IsCreature(hitCollider.gameObject))
                {
                    var hitCreature = hitCollider.GetComponent<CreatureCollider>()?.Creature;
                    if (hitCreature != null && hitCreature != _attackContext.Attacker)
                    {
                        // Apply damage to the creature
                        HandleCreatureHit(hitCreature);
                    }
                }
            }


            // Destroy the grenade
            Destroy(gameObject);
        }
    }
}