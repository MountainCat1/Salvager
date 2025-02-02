using System.Diagnostics;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InventoryEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemCountText;
        [SerializeField] private Image itemImage;
        
        private ItemBehaviour _item;
        private Creature _creature;
         
        public void SetItem(ItemBehaviour item, Creature creature)
        {
            itemNameText.text = item.Name;
            itemImage.sprite = item.Icon;
            
            _item = item;
            _creature = creature;
            
            itemCountText.text = item.Stackable ? item.Count.ToString() : "";
        }
        
        public void UseItem()
        {
            // Use item
            _item.Use(new ItemUseContext()
            {
                Creature = _creature
            });
        }
    }
}