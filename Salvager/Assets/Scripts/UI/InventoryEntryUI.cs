using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InventoryEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private Image itemImage;
        
        public void SetItem(ItemBehaviour item)
        {
            itemNameText.text = item.Name;
            itemImage.sprite = item.Icon;
        }
    }
}