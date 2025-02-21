using System;
using System.Linq;
using Zenject;

namespace Managers
{
    public interface IItemDescriptionManager
    {
        string GetDescription(ItemData item);
    }

    public class ItemDescriptionManager : IItemDescriptionManager
    {
        [Inject] private IItemManager _itemManager;
        
        public string GetDescription(ItemData data)
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
            var attackSpeedModifierString = GetWeaponModifierString(data, WeaponPropertyModifiers.AttackSpeed, weapon.BaseAttackSpeed);

            return $"Damage: {weapon.BaseDamage} {damageModifierString}\n" +
                   $"Range: {weapon.Range} {rangeModifierString}\n" +
                   $"Attack Speed: {weapon.BaseAttackSpeed} {attackSpeedModifierString}\n" +
                   $"Value {weapon.BaseCost}$";
        }

        private string GetWeaponModifierString(ItemData data, WeaponPropertyModifiers weaponPropertyModifier, float baseValue)
        {
            var modifier = data.GetChange(weaponPropertyModifier, baseValue);

            var modifierString = MathF.Abs(modifier) > 0.05f ? $" ({WrapInColor(modifier)})" : string.Empty;
            return modifierString;
        }

        private string WrapInColor(float f)
        {
            return f > 0 ? $"<color=green>+{f:F2}</color>" : $"<color=red>{f:F2}</color>";
        }

    }
}