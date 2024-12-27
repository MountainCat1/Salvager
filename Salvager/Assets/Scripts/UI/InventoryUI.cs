using TMPro;
using UI.Abstractions;
using UnityEngine;

namespace UI
{
    public class InventoryUI : PopupUI
    {
        [SerializeField] private InventoryEntryUI inventoryEntryUIPrefab;
        [SerializeField] private Transform entriesParent;
        [SerializeField] private TextMeshProUGUI creatureNameText;
        
        public void SetCreature(Creature creature)
        {
            foreach (Transform child in entriesParent)
            {
                Destroy(child.gameObject);
            }
            
            creatureNameText.text = creature.name;
            
            foreach (var item in creature.Inventory.Items)
            {
                var inventoryEntryUI = Instantiate(inventoryEntryUIPrefab, entriesParent);
                inventoryEntryUI.SetItem(item);
            }
        }
    }
}