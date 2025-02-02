using Items;
using UnityEngine;

public class ContainerObject : InteractableObject
{
    [SerializeField] private ItemBehaviour itemBehaviour = null!;
    
    protected override void OnInteractionComplete(Interaction interaction)
    {
        base.OnInteractionComplete(interaction);
        
        var creature = interaction.Creature;
        
        creature.Inventory.AddItemFromPrefab(itemBehaviour);
    }
}