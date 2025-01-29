using Managers;
using UnityEngine;
using Zenject;

namespace Interactables
{
    public class SignalObject : InteractableObject
    {
        [Inject] private ISignalManager _signalManager;
        
        [SerializeField] private Signal signal;
        
        protected override void OnInteractionComplete(Interaction interaction)
        {
            base.OnInteractionComplete(interaction);

            Debug.Log($"{name} sends signal {signal}");
            _signalManager.Signal(signal);
        }
    }
}