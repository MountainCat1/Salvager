using Godot;

public partial class Interactable : Entity, IInteractable
{
    private CreatureInteraction? _interaction;

    [Export] private float _interactionTime = 1f;

    public virtual bool CanInteract(Creature creature)
    {
        return true;
    }

    public virtual CreatureInteraction Interact(Creature creature, double deltaTime)
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

        _interaction.Progress(deltaTime);

        return returnInteraction;
    }

    private CreatureInteraction CreateInteraction(Creature creature)
    {
        _interaction = new CreatureInteraction(creature, _interactionTime);

        _interaction.Completed += () => { _interaction = null; };

        _interaction!.Canceled += () => { _interaction = null; };

        return _interaction;
    }

    public bool Occupied => _interaction != null;
}