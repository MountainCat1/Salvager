using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Managers
{
    public interface IJuiceManager
    {
        public event Action JuiceChanged;
        public decimal Juice { get; }
        float ConsumptionRate { get; }
        public void Initialize(IList<Creature> creatures, decimal juice);
    }

    public class JuiceManager : MonoBehaviour, IJuiceManager
    {
        public event Action JuiceChanged;
        public decimal Juice { get; private set; }

        public float ConsumptionRate => consumerConsumptionRate * _creatures?.Count(x => x.gameObject.activeInHierarchy) ?? 0;

        [Inject] private ICreatureManager _creatureManager;

        [SerializeField] private float consumerConsumptionRate = 1;
        [SerializeField] private float damageRate = 0.25f;

        private List<Creature> _creatures;

        public void Initialize(IList<Creature> creatures, decimal juice)
        {
            Juice = juice;
            JuiceChanged?.Invoke();
            
            _creatures = new List<Creature>(creatures);

            foreach (var creature in creatures)
            {
                creature.Health.Death += OnCreatureDeath;
            }

            StartCoroutine(DamageRoutine());
        }

        private IEnumerator DamageRoutine()
        {
            while (true)
            {
                if (Juice > 0)
                {
                    yield return new WaitForSeconds(1);
                    continue;
                }

                foreach (var creature in _creatures.Where(x => x.enabled).ToArray())
                {
                    creature.Health.Damage(new HitContext()
                    {
                        Damage = damageRate,
                        Attacker = null,
                        PushFactor = 0,
                        Target = creature
                    });
                }
                
                yield return new WaitForSeconds(1);
            }
        }

        private void OnCreatureDeath(DeathContext ctx)
        {
            _creatures.Remove(ctx.KilledEntity as Creature);
        }

        private void Update()
        {
            Juice -= (decimal)(ConsumptionRate * Time.deltaTime);
            if(Juice < 0)
                Juice = 0;
            JuiceChanged?.Invoke();
        }
    }
}