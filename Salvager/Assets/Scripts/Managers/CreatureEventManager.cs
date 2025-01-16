using System;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class CreatureEventManager : MonoBehaviour
    {
        [Inject] private ICreatureManager _creatureManager;
        [Inject] private IPopupManager _popupManager;

        [Inject] 
        private void Construct()
        {
            _creatureManager.CreatureSpawned += OnCreatureSpawned;
        }

        private void OnCreatureSpawned(Creature creature)
        {
            RegisterCreatureEvents(creature);   
        }
        
        private void RegisterCreatureEvents(Creature creature)
        {
           creature.Death += OnCreatureDeath;
        }

        
        // Event Handlers
        private void OnCreatureDeath(DeathContext ctx)
        {
            if (ctx.Killer == null)
            {
                Debug.LogWarning("Killer is null in DeathContext");
                return;
            }
            if(ctx.Creature == null)
                throw new NullReferenceException("Creature is null in DeathContext");
            
            var creature = ctx.Creature;
            var killer = ctx.Killer;

            killer.AwardXp(creature.XpAmount);
            
            Debug.Log($"{creature.name} was killed by {killer.name}");
        }
    }
}