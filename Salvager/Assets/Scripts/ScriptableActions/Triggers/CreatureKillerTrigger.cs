using System;
using UnityEngine;

namespace Triggers
{
    public class CreatureKillerTrigger : TriggerBase
    {
        [field: SerializeField] private Creature creature;

        private void Start()
        {
            creature.Death += OnCreatureDeath;
        }

        private void OnCreatureDeath(DeathContext obj)
        {
            RunActions();
        }
    }
}