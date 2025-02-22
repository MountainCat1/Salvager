using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using Managers;
using UnityEngine;
using Utilities;
using Zenject;
using Random = UnityEngine.Random;

namespace LevelSelector.Managers
{
    public interface IUpgradeManager
    {
        public void PlayerBuyUpgrade(ItemData item);
        void UpgradeItem(ItemData itemData);
        void ScrapItem(ItemData itemData);
        public int GetScrapValue(ItemData itemData);
        public int GetUpgradeCost(ItemData selectedItem);
        bool CanUpgrade(ItemData selectedItem);
        bool CanScrap(ItemData selectedItem);
    }

    public class UpgradeManager : MonoBehaviour, IUpgradeManager
    {
        [SerializeField] private float goodUpgradeChance = 1f;
        [SerializeField] private float badUpgradeChance = 1f;
        [SerializeField] private float normalUpgradeChance = 2f;
        
        [SerializeField] private float modifierMaxValue = 0.5f;
        [SerializeField] private float modifierMinValue = 0.1f;
        
        [Inject] private ICrewManager _crewManager;
        [Inject] private IItemManager _itemManager;

        [SerializeField] private ItemBehaviour scrapItemPrefab;
        
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
            {
                NormalUpgrade(item);
            }
        }
        
        public void PlayerBuyUpgrade(ItemData item)
        {
            if (!CanUpgrade(item))
                return;

            // Remove scrap cost
            var scrapCost = GetUpgradeCost(item);
            var scrapCostData = ItemData.FromPrefabItem(scrapItemPrefab);
            scrapCostData.Count = scrapCost;
                        
            _crewManager.Inventory.RemoveItem(scrapCostData);
            
            UpgradeItem(item);
        }

        public void ScrapItem(ItemData itemData)
        {
            if (itemData.Type != ItemType.Weapon)
            {
                Debug.LogError($"Only weapons can be scrapped, was trying to scrap {itemData.Identifier}");
                return;
            }
            
            var scrapValue = GetScrapValue(itemData);

            var scrap = ItemData.FromPrefabItem(scrapItemPrefab);
            scrap.Count = scrapValue;
            
            _crewManager.Inventory.AddItem(scrap);
            _crewManager.Inventory.RemoveItem(itemData);
        }

        public int GetScrapValue(ItemData itemData)
        {
            return (int)(_itemManager.GetValue(itemData) / 10) + 1;
        }

        public int GetUpgradeCost(ItemData selectedItem)
        {
            return 1 + selectedItem.Modifiers.Count;
        }

        public bool CanUpgrade(ItemData selectedItem)
        {
            if (selectedItem.Type != ItemType.Weapon)
                return false;
            
            var scrap = _crewManager.Inventory.GetItem(scrapItemPrefab.GetIdentifier())?.Count ?? 0;
            
            return scrap >= GetUpgradeCost(selectedItem);
        }

        public bool CanScrap(ItemData selectedItem)
        {
            return selectedItem.Type == ItemType.Weapon;
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
            // Add 2 positive and 1 negative modifier
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