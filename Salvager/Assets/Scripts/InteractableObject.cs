using JetBrains.Annotations;
using Managers;
using UnityEngine;
using Zenject;

public class InteractableObject : Entity, IInteractable
{
    [Inject] private ISoundPlayer _soundPlayer = null!;

    [CanBeNull] private Interaction _interaction;

    [SerializeField] private float interactionTime = 1f;
    [SerializeField] private AudioClip interactionSound;


    public virtual bool CanInteract(Creature creature)
    {
        return true;
    }

    public virtual Interaction Interact(Creature creature, float deltaTime)
    {
        if (_interaction == null)
        {
            _interaction = CreateInteraction(creature);
            return _interaction;
        }

        if (_interaction.Creature != creature)
        {
            _interaction.Cancel();
            _interaction = CreateInteraction(creature);
            return _interaction;
        }

        var returnInteraction = _interaction; // We make this temp so when it becomes null on OnCancel or OnComplete
        // we won't lose the reference 

        _interaction.Progress((decimal)deltaTime);

        return returnInteraction;
    }

    private Interaction CreateInteraction(Creature creature)
    {
        _interaction = new Interaction(creature, interactionTime);

        _interaction.Completed += () =>
        {
            _interaction = null;
            
            if (interactionSound)
                _soundPlayer.PlaySound(interactionSound, Position);
        };
        _interaction!.Canceled += () => { _interaction = null; };

        return _interaction;
    }

    public Vector2 Position => transform.position;
    public bool Occupied => _interaction != null;
}