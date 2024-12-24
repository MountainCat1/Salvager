using Items;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class InventoryEntryDisplay : MonoBehaviour
    {
        [Inject] private IPlayerCharacterProvider _playerProvider;
        
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI name;
        
        private ItemBehaviour _item;
        
        public void Initialize(ItemBehaviour item)
        {
            icon.sprite = item.Icon;
            name.text = item.Name;
            _item = item;
        }

        public void UseItem()
        {
            _playerProvider.Get().UseItem(_item);
        }
    }
}