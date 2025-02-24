using Items;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class InventoryEntryUI : MonoBehaviour
    {
        [Inject] private IItemDescriptionManager _itemDescriptionManager;
        
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemCountText;
        [SerializeField] private Image itemImage;
        
        private ItemBehaviour _item;
        private Creature _creature;
         
        public void SetItem(ItemBehaviour item, Creature creature)
        {
            itemNameText.text = _itemDescriptionManager.GetInfoName(item.GetData());
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