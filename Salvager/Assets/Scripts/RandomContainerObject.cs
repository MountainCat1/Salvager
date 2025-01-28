using Items;
using ScriptableObjects;
using UI;
using UnityEngine;
using Zenject;

public class RandomContainerObject : InteractableObject
{
    [Inject] private IFloatingTextManager _floatingTextManager;
    
    [SerializeField] private LootTable lootTable = null!;
    
    private ItemBehaviour _itemBehaviour;

    protected override void Awake()
    {
        // base.Awake(); // TODO: revert this change
        
        if (lootTable == null)
        {
            Debug.LogError("Loot table is not set");
            return;
        }
        
        _itemBehaviour = lootTable.GetRandomItem();

        if (_itemBehaviour == null) // we randomly chose empty item
        {
            Used = true; // we don't want to interact with this object anymore
        }
    }


    protected override void OnInteractionComplete(Interaction interaction)
    {
        base.OnInteractionComplete(interaction);
        
        var creature = interaction.Creature;
        
        creature.Inventory.AddItem(_itemBehaviour);
        
        _floatingTextManager.SpawnFloatingText(transform.position, _itemBehaviour.Name, Color.green);
    }
}