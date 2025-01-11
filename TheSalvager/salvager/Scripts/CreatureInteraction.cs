
using System;
using System.Diagnostics;
using Godot;

public interface IInteractable
{
    bool CanInteract(Creature creature);
    CreatureInteraction Interact(Creature creature, double deltaTime);
    public Vector2 Position { get; }
    bool Occupied { get; }
}

public enum InteractionStatus
{
    Created,
    InProgress,
    Completed,
    Canceled
}

public class CreatureInteraction
{
    public event Action? Completed;
    public event Action? Canceled;
    public event Action? Ended;
    
    public InteractionStatus Status { get; private set; } = InteractionStatus.Created;

    public double CurrentProgress { get; private set; }
    public Creature Creature { get; }
    public double InteractionTime { get; }

    public CreatureInteraction(Creature creature, float interactionTime)
    {
        Creature = creature;
        InteractionTime = interactionTime;
    }
    
    public void Progress(double delta)
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
        
        GD.Print($"{Creature.Name} canceled the interaction.");
    }

    private void OnComplete()
    {
        Status = InteractionStatus.Completed;
        Completed?.Invoke();
        Ended?.Invoke();

        GD.Print($"{Creature.Name} completed the interaction.");
    }
}