using Managers;
using Zenject;

namespace Interactables
{
    public class ExitObject : InteractableObject
    {
        [Inject] private ISignalManager _signalManager;
        
        protected override void OnInteractionComplete(Interaction interaction)
        {
            base.OnInteractionComplete(interaction);
            
            interaction.Creature.gameObject.SetActive(false);
            
            _signalManager.Signal(Signal.CreatureExited);
        }
    }
}