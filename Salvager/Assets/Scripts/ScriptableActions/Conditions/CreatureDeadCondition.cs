using System;
using UnityEngine;

namespace ScriptableActions.Conditions
{
    public class CreatureDeadCondition : ConditionBase
    {
        [SerializeField] private Creature creature;

        private bool _creatureDied = false;

        private void Start()
        {
            creature.Death += OnCreatureDied;
        }

        private void OnCreatureDied(DeathContext ctx)
        {
            _creatureDied = true;
        }

        protected override bool Check()
        {
            return _creatureDied;
        }
    }
}