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
    }


    protected override void OnInteractionComplete(Interaction interaction)
    {
        base.OnInteractionComplete(interaction);
        
        var creature = interaction.Creature;
        
        if(_itemBehaviour == null)
        {
            _floatingTextManager.SpawnFloatingText(transform.position, "Empty", FloatingTextType.Miss);
            return;
        }
        
        creature.Inventory.AddItemFromPrefab(_itemBehaviour);
        _floatingTextManager.SpawnFloatingText(transform.position, _itemBehaviour.Name, FloatingTextType.InteractionCompleted);
    }
}