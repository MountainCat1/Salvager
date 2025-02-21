using System;
using System.Linq;
using Data;
using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Zenject;

namespace UI
{
    [RequireComponent(typeof(TooltipTrigger))]
    public class ItemEntryUI : MonoBehaviour
    {
        [Inject] private IItemManager _itemManager;
        [Inject] private IItemDescriptionManager _itemDescriptionManager;
        [Inject] private IDataResolver _dataResolver;

        [SerializeField] private Image iconDisplay;
        [SerializeField] private TextMeshProUGUI nameDisplay;
        [SerializeField] private TextMeshProUGUI descriptionDisplay;
        [SerializeField] private TextMeshProUGUI amountDisplay;

        protected ItemData ItemData;

        private Action<ItemData> _buttonClickCallback;

        private TooltipTrigger _toolTip;

        private void Awake()
        {
            _toolTip = GetComponent<TooltipTrigger>();
        }

        public void Set(ItemData item, Action<ItemData> transferCallback)
        {
            nameDisplay.text = item.Prefab.Name;
            amountDisplay.text = item.Count == 1 ? string.Empty : $"x{item.Count}";
            iconDisplay.sprite = item.Prefab.Icon;
            // descriptionDisplay.text = item.Description;

            ItemData = item;
            _buttonClickCallback = transferCallback;

            _toolTip.text = $"<b>{item.Prefab.Name}</b>\n-----\n{_itemDescriptionManager.GetDescription(item)}";
        }

        public void RunCallback()
        {
            _buttonClickCallback?.Invoke(ItemData);
        }
    }
}