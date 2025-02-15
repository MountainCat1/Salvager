using Data;
using Managers;
using Managers.LevelSelector;
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

        [SerializeField] private ItemShopEntryUI itemEntryUIPrefab;

        [SerializeField] private Transform shopInventoryContainer;
        [SerializeField] private Transform crewInventoryContainer;
        
        [SerializeField] private AudioClip buySound;
        [SerializeField] private AudioClip sellSound;

        private UISlide _uiSlide;
        private ShopData _shopData;

        private void Start()
        {
            _crewManager.Changed += UpdateUI;
            _crewManager.SelectedCreature += _ => _uiSlide.HidePanel();
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
                var itemEntryGo = _diContainer.InstantiatePrefab(itemEntryUIPrefab, shopInventoryContainer);
                var itemEntry = itemEntryGo.GetComponent<ItemShopEntryUI>();
                itemEntry.Set(item, BuyItem);
                itemEntry.SetShopData(_shopData);

                var button = itemEntryGo.GetComponent<Button>();
                button.interactable = _crewManager.Resources.Money >= _shopData.GetBuyPrice(item);
                
                itemEntryGo.GetComponent<ButtonSoundUI>().audioClip = buySound;
            }

            foreach (var item in _crewManager.Inventory.Items)
            {
                var itemEntryGo = _diContainer.InstantiatePrefab(itemEntryUIPrefab, crewInventoryContainer);
                var itemEntry = itemEntryGo.GetComponent<ItemShopEntryUI>();
                itemEntry.Set(item, SellItem);
                itemEntry.SetShopData(_shopData);
                
                itemEntry.GetComponent<ButtonSoundUI>().audioClip = sellSound;
            }
        }

        private void SellItem(ItemData item)
        {
            _crewManager.Inventory.TransferItem(item, _shopData.inventory);

            _crewManager.Resources.AddMoney(_shopData.GetSellPrice(item));

            UpdateUI();

            _dataManager.SaveData();
        }

        private void BuyItem(ItemData item)
        {
            _shopData.inventory.TransferItem(item, _crewManager.Inventory);

            _crewManager.Resources.AddMoney(-1 * _shopData.GetBuyPrice(item));

            UpdateUI();

            _dataManager.SaveData();
        }
    }
}