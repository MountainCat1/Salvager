using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ItemEntryUI : MonoBehaviour
    {
        [SerializeField] private Image iconDisplay;
        [SerializeField] private TextMeshProUGUI nameDisplay;
        [SerializeField] private TextMeshProUGUI descriptionDisplay;
        [SerializeField] private TextMeshProUGUI amountDisplay;
        
        private ItemData _itemData;
        private Action<ItemData> _transferCallback;

        public void Set(ItemData item, Action<ItemData> transferCallback)
        {
            iconDisplay.sprite = item.Icon;
            nameDisplay.text = item.Name;
            amountDisplay.text = item.Count == 1 ? string.Empty : item.Count.ToString();
            // descriptionDisplay.text = item.Description;
            
            _itemData = item;
            _transferCallback = transferCallback;
        }
        
        public void Transfer()
        {
            _transferCallback?.Invoke(_itemData);
        }
    }
}