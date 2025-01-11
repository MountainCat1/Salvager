using Godot;

namespace UI;

public partial class InteractionProgressBar : ProgressBar
{
    [Export] private Creature _creature = null!;

    private void Start()
    {
        _creature.Interacted += OnInteracted;
            
        Visible = false;
    }

    private void OnInteracted(CreatureInteraction interaction)
    {
        UpdateProgressBar(interaction);
    }

    private void UpdateProgressBar(CreatureInteraction interaction)
    {
        Value = (float)interaction.CurrentProgress;
        MaxValue = (float)interaction.InteractionTime;
        
        Visible = interaction.Status == InteractionStatus.InProgress;
    }
}