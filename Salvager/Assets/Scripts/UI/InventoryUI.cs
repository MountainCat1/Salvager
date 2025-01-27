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
        
        private Creature _creature;
        public Creature Creature => _creature;

        public void SetCreature(Creature creature)
        {
            _creature = creature;
            
            creature.Inventory.Changed += UpdateInventory;
            
           UpdateInventory();
        }

        private void UpdateInventory()
        {
            foreach (Transform child in entriesParent)
            {
                Destroy(child.gameObject);
            }
            
            creatureNameText.text = _creature.name;
            
            foreach (var item in _creature.Inventory.Items)
            {
                var inventoryEntryUI = Instantiate(inventoryEntryUIPrefab, entriesParent);
                inventoryEntryUI.SetItem(item);
            }
        }
    }
}