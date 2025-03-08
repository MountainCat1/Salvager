using System.Collections.Generic;
using Managers;
using UnityEngine;
using Zenject;

namespace Triggers
{
    public class InteractableTrigger : TriggerBase
    {
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