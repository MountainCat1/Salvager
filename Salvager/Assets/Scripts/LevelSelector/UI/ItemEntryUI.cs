using System;
using Data;
using Items;
using Managers;
using TMPro;
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
        [Inject] private IDataResolver _dataResolver;
        
        [SerializeField] private Image iconDisplay;
        [SerializeField] private TextMeshProUGUI nameDisplay;
        [SerializeField] private TextMeshProUGUI descriptionDisplay;
        [SerializeField] private TextMeshProUGUI amountDisplay;

        private ItemData _itemData;
        private Action<ItemData> _transferCallback;

        private TooltipTrigger _toolTip;

        private void Awake()
        {
            _toolTip = GetComponent<TooltipTrigger>();
        }

        public void Set(ItemData item, Action<ItemData> transferCallback)
        {
            iconDisplay.sprite = _dataResolver.ResolveItemIcon(item.Icon);
            nameDisplay.text = item.Name;
            amountDisplay.text = item.Count == 1 ? string.Empty : item.Count.ToString();
            // descriptionDisplay.text = item.Description;

            _itemData = item;
            _transferCallback = transferCallback;

            _toolTip.text = $"<b>{item.Name}</b>\n-----\n{GetDescription(item)}";
        }

        private string GetDescription(ItemData data)
        {
            var item = _itemManager.GetItemPrefab(data.Identifier); // TODO: we should not rely on prefab i think but not sure

            switch (item)
            {
                case Weapon weapon:
                    return $"Damage: {weapon.BaseDamage}\nRange: {weapon.Range}\nAttack Speed: {weapon.BaseAttackSpeed}";
                default:
                    return item.Description;
            }
        }

        public void Transfer()
        {
            _transferCallback?.Invoke(_itemData);
        }
    }
}