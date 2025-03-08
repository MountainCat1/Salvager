using System.Collections.Generic;
using Managers;
using UnityEngine;
using Zenject;

namespace Triggers
{
    public class InteractableTrigger : TriggerBase
    {
    public class InteractableTrigger : TriggerBase
    {
        [SerializeField] private InteractableObject interactableObject;
        // ... rest of the class remains unchanged
    
        [SerializeField] private InteractableObject interactableObject;
        
        protected override void Start()
        {
            base.Start();
            
            interactableObject.InteractCompleted += OnInteractCompleted;
        }

        private void OnInteractCompleted(Creature obj)
        {
            RunActions();
        }
    }

}