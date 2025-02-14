using Data;
using Managers;
using Managers.LevelSelector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class ShopUI : MonoBehaviour
    {
        [Inject] private ICrewManager _crewManager;
        [Inject] private IDataManager _dataManager;
        [Inject] private IRegionManager _regionManager;
        [Inject] private DiContainer _diContainer;

        [SerializeField] private ItemEntryUI itemEntryUIPrefab;

        [SerializeField] private Transform shopInventoryContainer;
        [SerializeField] private Transform crewInventoryContainer;

        private UISlide _uiSlide;
        private ShopData _shopData;

        private void Start()
        {
            _crewManager.Changed += UpdateUI;
            _uiSlide = GetComponent<UISlide>();
        }

        private void UpdateUI()
        {
            foreach (Transform child in shopInventoryContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in crewInventoryContainer)
            {
                Destroy(child.gameObject);
            }

            _shopData = _regionManager.Region.GetLocation(_crewManager.CurrentLocationId)?.ShopData;
            
            if(_shopData == null)
                return;

            foreach (var item in _shopData.inventory.Items)
            {
                var itemEntry = _diContainer.InstantiatePrefab(itemEntryUIPrefab, shopInventoryContainer);
                itemEntry.GetComponent<ItemEntryUI>().Set(item, BuyItem);

                var button = itemEntry.GetComponent<Button>();
                button.interactable = _crewManager.Resources.Money >= item.Value;
            }

            foreach (var item in _crewManager.Inventory.Items)
            {
                var itemEntry = _diContainer.InstantiatePrefab(itemEntryUIPrefab, crewInventoryContainer);
                itemEntry.GetComponent<ItemEntryUI>().Set(item, SellItem);
            }
        }

        private void SellItem(ItemData item)
        {
            _crewManager.Inventory.RemoveItem(item);
            _shopData.inventory.AddItem(item);

            _crewManager.Resources.AddMoney(item.Value);

            UpdateUI();

            _dataManager.SaveData();
        }

        private void BuyItem(ItemData item)
        {
            _crewManager.Inventory.AddItem(item);
            _shopData.inventory.RemoveItem(item);

            _crewManager.Resources.AddMoney(-item.Value);

            UpdateUI();

            _dataManager.SaveData();
        }
    }
}