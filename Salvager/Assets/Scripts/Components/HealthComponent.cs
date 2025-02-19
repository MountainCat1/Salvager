namespace Components
{
    using System;
    using UnityEngine;

    public interface IDamageable
    {
        HealthComponent Health { get; }
    }

    public class HealthComponent : MonoBehaviour, IReadonlyRangedValue
    {
        // Events
        public event Action<DeathContext> Death;
        public event Action ValueChanged;
        public event Action<HitContext> Hit;

        // Settings
        [Header("Health Settings")] [SerializeField]
        private float maxHealth = 10;

        // Accessors
        public float CurrentValue => _rangeValue.CurrentValue;
        public float MinValue => _rangeValue.MinValue;
        public float MaxValue => _rangeValue.MaxValue;

        public bool Alive => _rangeValue.CurrentValue > _rangeValue.MinValue;

        // Fields
        private RangedValue _rangeValue;

        private void Awake()
        {
            _rangeValue = new RangedValue(maxHealth, 0f, maxHealth);
        }

        public void Damage(HitContext ctx)
        {
            if (!gameObject.activeInHierarchy)
            {
                Debug.LogError("Trying to damage a disabled object");
                return;
            }

            ctx.ValidateAndLog();

            _rangeValue.CurrentValue -= ctx.Damage;
            ValueChanged?.Invoke();

            Hit?.Invoke(ctx);

            if (_rangeValue.CurrentValue <= _rangeValue.MinValue)
            {
                InvokeDeath(ctx.Attacker);
            }
        }

        public void Heal(float healAmount)
        {
            if (!gameObject.activeInHierarchy)
            {
                Debug.LogError("Trying to heal a disabled object");
                return;
            }
            
            _rangeValue.CurrentValue += healAmount;
            ValueChanged?.Invoke();
        }

        private void InvokeDeath(Creature lastAttackedBy)
        {
            var deathContext = new DeathContext()
            {
                Killer = lastAttackedBy,
                KilledEntity = GetComponent<Entity>() // Optional: Associate with owning entity
            };

            Death?.Invoke(deathContext);
            Destroy(gameObject); // Optional: Destroy the GameObject
        }
    }
}