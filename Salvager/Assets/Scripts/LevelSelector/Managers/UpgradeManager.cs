using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace LevelSelector.Managers
{
    public interface IUpgradeManager
    {
        void UpgradeItem(ItemData itemData);
    }

    public class UpgradeManager : MonoBehaviour, IUpgradeManager
    {
        [SerializeField] private float goodUpgradeChance = 1f;
        [SerializeField] private float badUpgradeChance = 1f;
        [SerializeField] private float normalUpgradeChance = 2f;


        [SerializeField] private float modifierMaxValue = 0.5f;
        [SerializeField] private float modifierMinValue = 0.1f;

        public void UpgradeItem(ItemData item)
        {
            float totalChance = goodUpgradeChance + badUpgradeChance + normalUpgradeChance;
            float randomValue = Random.Range(0f, totalChance);

            if (randomValue <= goodUpgradeChance)
            {
                GoodUpgrade(item);
            }
            else if (randomValue <= goodUpgradeChance + badUpgradeChance)
            {
                BadUpgrade(item);
            }
            else
            {
                NormalUpgrade(item);
            }
        }

        public void GoodUpgrade(ItemData item)
        {
            // Add 2 positive modifiers
            item.Modifiers.Add(GetPositiveWeaponModifier(item));
            item.Modifiers.Add(GetPositiveWeaponModifier(item));
        }

        public void BadUpgrade(ItemData item)
        {
            // Add 1 negative modifier
            item.Modifiers.Add(GetNegativeWeaponModifier(item));
        }

        public void NormalUpgrade(ItemData item)
        {
            // Add 1 positive and 1 negative modifier
            item.Modifiers.Add(GetPositiveWeaponModifier(item));
            item.Modifiers.Add(GetPositiveWeaponModifier(item));
            item.Modifiers.Add(GetNegativeWeaponModifier(item));
        }

        private WeaponValueModifier GetNegativeWeaponModifier(ItemData item)
        {
            var modifier = GetPositiveWeaponModifier(item);
            modifier.Value *= -1;
            return modifier;
        }
        
        private WeaponValueModifier GetPositiveWeaponModifier(ItemData item)
        {
            var property = GetValidProperty(item);

            var modifier = new WeaponValueModifier()
            {
                Type = property.RandomElement(),
                Value = Random.Range(modifierMinValue, modifierMaxValue)
            };
            return modifier;
        }

        private ICollection<WeaponPropertyModifiers> GetValidProperty(ItemData item)
        {
            return Enum.GetValues(typeof(WeaponPropertyModifiers)).Cast<WeaponPropertyModifiers>().ToList();
        }
    }
}