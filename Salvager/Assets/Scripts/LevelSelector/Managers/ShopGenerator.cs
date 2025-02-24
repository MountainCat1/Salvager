using System.Collections.Generic;
using System.Linq;
using Items;
using Managers;
using ScriptableObjects;
using UnityEngine;
using Zenject;

namespace LevelSelector.Managers
{
    public interface IShopGenerator
    {
        public ShopData GenerateShop();
    }

    public class ShopGenerator : MonoBehaviour, IShopGenerator
    {
        [Inject] private IItemManager _itemManager;
        [Inject] private IUpgradeManager _upgradeManager;
        
        [SerializeField] private LootTable lootTable;

        private const int MinItems = 3;
        private const int MaxItems = 6;

        private const float ChanceForUpgrades = 0.5f;
        
        private const float MinPriceMultiplier = 0.7f;
        private const float MaxPriceMultiplier = 1.3f;
        
        public ShopData GenerateShop()
        {
            var shopData = new ShopData();
            
            var itemCount = Random.Range(MinItems, MaxItems);
            
            shopData.itemCount = itemCount;

            var items = new List<ItemData>();
            for (int i = 0; i < itemCount; i++)
            {
                var lootEntry = lootTable.GetRandomItem();
                
                var itemData = ItemData.FromPrefabItem(lootEntry.item);
                
                var count = Random.Range(lootEntry.minCount, lootEntry.maxCount);
                itemData.Count = count;
                
                items.Add(itemData);
            }

            shopData.inventory = new InventoryData()
            {
                Items = items
            };
            shopData.priceMultiplier = Random.Range(MinPriceMultiplier, MaxPriceMultiplier);

            foreach (var itemData in shopData.inventory.Items)
            {
                if(itemData.Type != ItemType.Weapon)
                    continue;
                if (Random.value > ChanceForUpgrades)
                    continue;
                
                
                _upgradeManager.UpgradeItem(itemData);
            }

            return shopData;
        }
    }
}