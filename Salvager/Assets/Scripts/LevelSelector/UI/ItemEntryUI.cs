using System;
using System.Linq;
using System.Resources;
using Data;
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

        protected ItemData ItemData;
        
        private Action<ItemData> _buttonClickCallback;

        private TooltipTrigger _toolTip;

        private void Awake()
        {
            _toolTip = GetComponent<TooltipTrigger>();
        }

        public void Set(ItemData item, Action<ItemData> transferCallback)
        {
            var itemBehaviour = _itemManager.GetItemPrefab(item.Identifier);
            iconDisplay.sprite = itemBehaviour.Icon;
            nameDisplay.text = item.Name;
            amountDisplay.text = item.Count == 1 ? string.Empty : $"x{item.Count}";
            // descriptionDisplay.text = item.Description;

            ItemData = item;
            _buttonClickCallback = transferCallback;

            _toolTip.text = $"<b>{item.Name}</b>\n-----\n{GetDescription(item)}";
        }

        private string GetDescription(ItemData data)
        {
            var item = _itemManager.GetItemPrefab(data.Identifier); // TODO: we should not rely on prefab i think but not sure

            switch (item)
            {
                case Weapon weapon:
                    return GetWeaponDescription(data, weapon);
                default:
                    return $"{item.Description}\nValue {data.Value}$";
            }
        }

        private string GetWeaponDescription(ItemData data, Weapon weapon)
        {
            var weaponBaseDamage = weapon.Damage;
            var damageModifierString = GetWeaponModifierString(data, WeaponPropertyModifiers.Damage, weaponBaseDamage);

            return $"Damage: {weaponBaseDamage} {damageModifierString}\nRange: {weapon.Range}\nAttack Speed: {weapon.AttackSpeed}\nValue {data.Value}$";
        }

        private string GetWeaponModifierString(ItemData data, WeaponPropertyModifiers modifierName, float baseValue)
        {
            var weaponModifiers = data.Modifiers.OfType<WeaponValueModifier>();
            var modifier = WeaponValueModifier.GetChange(weaponModifiers, modifierName, baseValue);
                    
            var damageModifierString = MathF.Abs(modifier) > 0.05f ? $" ({WrapInColor(modifier)})" : string.Empty;
            return damageModifierString;
        }

        private string WrapInColor(float f)
        {
            return f > 0 ? $"<color=green>+{f:F2}</color>" : $"<color=red>{f:F2}</color>";
        }

        public void RunCallback()
        {
            _buttonClickCallback?.Invoke(ItemData);
        }
    }
}