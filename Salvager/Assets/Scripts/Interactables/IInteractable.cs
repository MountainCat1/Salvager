using System;
using UnityEngine;

public interface IInteractable
{
    bool CanInteract(Creature creature);
    Interaction Interact(Creature creature, float deltaTime);
    public Vector2 Position { get; }
    bool IsInteractable { get; }
}

public enum InteractionStatus
{
    Created,
    InProgress,
    Completed,
    Canceled
}

public class Interaction
{
    public event Action Completed;
    public event Action Canceled;
    public event Action Ended;
    
    public InteractionStatus Status { get; private set; } = InteractionStatus.Created;

    public decimal CurrentProgress { get; private set; }
    public Creature Creature { get; }
    public decimal InteractionTime { get; }
    public string Message { get; }

    public Interaction(Creature creature, float interactionTime, string message = "")
    {
        Creature = creature;
        Message = message;
        InteractionTime = (decimal)interactionTime;
    }
    
    public void Progress(decimal delta)
    {
        if(Status != InteractionStatus.Created && Status != InteractionStatus.InProgress)
            throw new InvalidOperationException("Cannot progress a completed or canceled interaction.");
        
        Status = InteractionStatus.InProgress;
        
        if(delta < 0)
            throw new ArgumentOutOfRangeException(nameof(delta), "Delta must be positive.");
        
        CurrentProgress += delta;
        
        if (CurrentProgress >= InteractionTime)
        {
            OnComplete();
        }
    }
    
    public void Cancel()
    {
        Status = InteractionStatus.Canceled;
        Canceled?.Invoke();
        Ended?.Invoke();
        
        Debug.Log($"{Creature.name} canceled the interaction.");
    }

    private void OnComplete()
    {
        Status = InteractionStatus.Completed;
        Completed?.Invoke();
        Ended?.Invoke();

        Debug.Log($"{Creature.name} completed the interaction.");
    }
}