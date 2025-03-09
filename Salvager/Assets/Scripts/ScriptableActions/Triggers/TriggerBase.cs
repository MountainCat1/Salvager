using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableActions.Conditions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Triggers
{
    public class TriggerBase : MonoBehaviour
    {
        [field: SerializeField, HideInInspector] public List<ScriptableAction> Actions { get; private set; }
        [field: SerializeField, HideInInspector] public List<ConditionBase> Conditions { get; private set; }
        [field: SerializeField] public bool FireOnce { get; private set; } = true;
        [field: SerializeField] private bool actionObject = true;
        protected bool CanRun => !(FireOnce && HasFired);
        
        public bool HasFired { get; private set; }

        protected virtual void Start()
        {
            if (actionObject)
            {
                var actions = GetComponents<ScriptableAction>();

                foreach (var action in actions.Where(x => !Actions.Contains(x)))
                {
                    Actions.Add(action);
                }
                
                var conditions = GetComponents<ConditionBase>();
                
                foreach (var condition in conditions.Where(x => !Conditions.Contains(x)))
                {
                    Conditions.Add(condition);
                }
            }
        }

        protected void RunActions()
        {
            if (FireOnce && HasFired)
                return;
         
            if(!CheckConditions())
                return;
            
            HasFired = true;

            Debug.Log($"Trigger {name} fired");
            foreach (var action in Actions)
            {
                action.Execute();
            }
        }

        private bool CheckConditions()
        {
            foreach (var condition in Conditions)
            {
                if (!condition.Evaluate())
                    return false; // Return false if any condition fails
            }

            return true; // Return true if all conditions pass
        }
    }
}