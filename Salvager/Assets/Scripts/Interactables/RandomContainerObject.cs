using Items;
using Managers;
using ScriptableObjects;
using UI;
using UnityEngine;
using Zenject;

public class RandomContainerObject : InteractableObject
{
    [Inject] private IFloatingTextManager _floatingTextManager;
    [Inject] private IItemManager _itemManager;
    
    [SerializeField] private LootTable lootTable = null!;
    
    private LootTableEntry _loot;

    protected override void Awake()
    {
        // base.Awake(); // TODO: revert this change
        
        if (lootTable == null)
        {
            Debug.LogError("Loot table is not set");
            return;
        }
        
        _loot = lootTable.GetRandomItem();
    }


    protected override void OnInteractionComplete(Interaction interaction)
    {
        base.OnInteractionComplete(interaction);
        
        var creature = interaction.Creature;
        
        if(_loot.item is null)
        {
            _floatingTextManager.SpawnFloatingText(transform.position, "Empty", FloatingTextType.Miss);
            return;
        }
        
        var itemData = ItemData.FromPrefabItem(_loot.item);
        itemData.Count = Random.Range(_loot.minCount, _loot.maxCount);
        creature.Inventory.AddItem(itemData);
        
        
        var floatingText = itemData.Count == 1 
            ? itemData.Prefab.Name 
            : $"{itemData.Prefab.Name} x{itemData.Count}";
        
        _floatingTextManager.SpawnFloatingText(transform.position, floatingText, FloatingTextType.InteractionCompleted);
    }
    
    public ItemBehaviour GetRandomItem()
    {
        var itemData = ItemData.FromPrefabItem(_loot.item);
        itemData.Count = Random.Range(_loot.minCount, _loot.maxCount);
        
        var instantiatedItem = _itemManager.InstantiateItem(itemData);
        
        return instantiatedItem;
    }
}