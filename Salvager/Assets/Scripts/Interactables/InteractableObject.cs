using System;
using System.Collections.Generic;
using Interactables;
using Items;
using JetBrains.Annotations;
using Managers;
using UnityEngine;
using Zenject;

public class InteractableObject : PropEntity, IInteractable
{
    [Inject] private ISoundPlayer _soundPlayer = null!;
    [Inject] private ICursorManager _cursorManager = null!;

    [CanBeNull] private Interaction _interaction;

    [SerializeField] private float interactionTime = 1f;
    [SerializeField] private AudioClip interactionSound;
    [SerializeField] private AudioClip startInteractionSound;
    [SerializeField] private bool useOnce = true;
    [SerializeField] private string message = "Interacting...";
    [SerializeField] public List<ItemBehaviour> requiredItems;
    [SerializeField] public Texture2D cursor;
    
    // Accessors
    public bool IsInteractable => GetIsInteractable();
    public Vector2 Position => transform.position;
    public bool Occupied => _interaction != null;
    
    // Private Variables
    protected bool Used = false;

    // Methods

    private void OnMouseEnter()
    {
        if(cursor && IsInteractable && (!Used || !useOnce))
        {
            _cursorManager.SetCursor(this, cursor, CursorPriority.Interactable);   
        }
    }
    
    private void OnMouseExit()
    {
        _cursorManager.RemoveCursor(this);
    }


    private bool GetIsInteractable()
    {
        if(Used && useOnce)
            return false;
        
        return true;
    }

    public virtual bool CanInteract(Creature creature)
    {
        if(IsInteractable == false)
            return false;
        
        if(requiredItems != null)
        {
            foreach (var requiredItem in requiredItems)
            {
                if(creature.Inventory.HasItem(requiredItem.GetIdentifier()) == false)
                    return false;
            }
        }
        
        if(Occupied && _interaction!.Creature != creature && useOnce)
            return false;
        
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
        _interaction = new Interaction(creature, interactionTime, message);

        _interaction.Completed += () =>
        {
            if (interactionSound)
                _soundPlayer.PlaySound(interactionSound, Position);
            
            Used = true;

            try
            {
                OnInteractionComplete(_interaction);
            }
            finally
            {
                _interaction = null;
            }
        };
        _interaction!.Canceled += () => { _interaction = null; };

        if (startInteractionSound)
            _soundPlayer.PlaySound(startInteractionSound, Position);
        
        return _interaction;
    }

    protected virtual void OnInteractionComplete(Interaction interaction)
    {
        if(useOnce)
            _cursorManager.RemoveCursor(this);
    }
}