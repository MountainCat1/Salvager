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

            _toolTip.text = $"<b>{item.Prefab.Name}</b>\n-----\n{GetDescription(item)}";
        }

        private string GetDescription(ItemData data)
        {
            var item = _itemManager.GetItemPrefab(data
                .Identifier); // TODO: we should not rely on prefab i think but not sure

            switch (item)
            {
                case Weapon weapon:
                    return GetWeaponDescription(data, weapon);
                default:
                    return $"{item.Description}\nValue {data.Prefab.BaseCost}$";
            }
        }

        private string GetWeaponDescription(ItemData data, Weapon weapon)
        {
            var damageModifierString = GetWeaponModifierString(data, WeaponPropertyModifiers.Damage, weapon.BaseDamage);
            var rangeModifierString = GetWeaponModifierString(data, WeaponPropertyModifiers.Range, weapon.Range);
            var attackSpeedModifierString =
                GetWeaponModifierString(data, WeaponPropertyModifiers.AttackSpeed, weapon.BaseAttackSpeed);

            return $"Damage: {weapon.BaseDamage} {damageModifierString}\n" +
                   $"Range: {weapon.Range} {rangeModifierString}\n" +
                   $"Attack Speed: {weapon.BaseAttackSpeed} {attackSpeedModifierString}\n" +
                   $"Value {weapon.BaseCost}$";
        }

        private string GetWeaponModifierString(ItemData data, WeaponPropertyModifiers modifierName, float baseValue)
        {
            var weaponModifiers = data.Modifiers.OfType<WeaponValueModifier>();
            var modifier = data.GetChange(modifierName, baseValue);

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