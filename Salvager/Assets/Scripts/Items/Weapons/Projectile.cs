﻿using System;
using UnityEngine;
using Utilities;

namespace Items.Weapons
{
    public class Projectile : MonoBehaviour
    {
        public event Action<Creature, AttackContext> Hit;

        [SerializeField] private ColliderEventProducer colliderEventProducer;
        
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
            if(!_initialized)
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

            var hitCreature = other.GetComponent<Creature>();

            if (hitCreature == null)
            {
                Debug.LogError("Projectile hit something that is not a creature");
                return;
            }

            if (hitCreature == _attackContext.Attacker)
                return;

            try
            {
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