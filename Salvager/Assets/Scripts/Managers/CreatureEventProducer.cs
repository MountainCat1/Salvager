using System;
using Zenject;

namespace Managers
{
    public interface ICreatureEventProducer
    {
        public event Action<Creature, HitContext> CreatureHit;
        public event Action<Creature, DeathContext> CreatureDied;

    }
    
    public class CreatureEventProducer : ICreatureEventProducer
    {
        [Inject] private ICreatureManager _creatureManager;
        
        public event Action<Creature, HitContext> CreatureHit;
        public event Action<Creature, DeathContext> CreatureDied;
        
        [Inject]
        private void Initialize()
        {
            _creatureManager.CreatureSpawned += OnCreatureSpawned;

            foreach (var creature in _creatureManager.GetCreatures())
            {
                OnCreatureSpawned(creature);
            }
        }

        private void OnCreatureSpawned(Creature creature)
        {
            creature.Health.Hit += (HitContext context) =>
            {
                CreatureHit?.Invoke(creature, context);
            };
            
            creature.Health.Death += (DeathContext context) =>
            {
                CreatureDied?.Invoke(creature, context);
            };
        }
    }
}