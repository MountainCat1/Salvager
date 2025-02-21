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
        [Inject] private IItemManager _itemManager;
        [Inject] private IRegionManager _regionManager;
        [Inject] private DiContainer _diContainer;

        [SerializeField] private ItemShopEntryUI itemEntryUIPrefab;

        [SerializeField] private Transform shopInventoryContainer;
        [SerializeField] private Transform crewInventoryContainer;

        [SerializeField] private AudioClip buySound;
        [SerializeField] private AudioClip sellSound;

        [SerializeField] private TextMeshProUGUI fuelPrice;
        [SerializeField] private Button buyFuelButton;
        [SerializeField] private TextMeshProUGUI juicePrice;
        [SerializeField] private Button buyJuiceButton;

        private UISlide _uiSlide;
        private ShopData _shopData;

        private void Start()
        {
            _crewManager.Changed += UpdateUI;
            _crewManager.SelectedCreature += _ => _uiSlide.HidePanel();
            _uiSlide = GetComponent<UISlide>();

            UpdateUI();
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

            if (_shopData == null)
                return;

            fuelPrice.text = $"{_shopData.GetFuelPrice()}$";
            buyFuelButton.interactable = _crewManager.Resources.Money >= _shopData.GetFuelPrice();
            
            juicePrice.text = $"{_shopData.GetJuicePrice()}$";
            buyJuiceButton.interactable = _crewManager.Resources.Money >= _shopData.GetJuicePrice();

            foreach (var item in _shopData.inventory.Items)
            {
                var itemEntryGo = _diContainer.InstantiatePrefab(itemEntryUIPrefab, shopInventoryContainer);
                var itemEntry = itemEntryGo.GetComponent<ItemShopEntryUI>();
                itemEntry.Set(item, BuyItem);
                itemEntry.SetShopData(_shopData);

                var button = itemEntryGo.GetComponent<Button>();
                button.interactable = _crewManager.Resources.Money >= _shopData.GetBuyPrice(_itemManager.GetValue(item));

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
            var transferItemCount =
                _crewManager.Inventory.TransferItem(item, _shopData.inventory, GetTransferItemCount());

            _crewManager.Resources.AddMoney(_shopData.GetSellPrice(_itemManager.GetValue(item)) * transferItemCount);

            UpdateUI();

            _dataManager.SaveData();
        }

        private void BuyItem(ItemData item)
        {
            var transferItemCount =
                _shopData.inventory.TransferItem(item, _crewManager.Inventory, GetTransferItemCount());

            _crewManager.Resources.AddMoney(-1 * (_itemManager.GetValue(item) * transferItemCount));

            UpdateUI();

            _dataManager.SaveData();
        }

        public void BuyFuel()
        {
            var price = _shopData.GetFuelPrice();
            if (_crewManager.Resources.Money < price)
            {
                Debug.LogError("Trying to buy fuel without enough money");
                return;
            }

            _crewManager.Resources.AddMoney(-1 * price);
            _crewManager.Resources.AddFuel(1);
        }
        
        public void BuyJuice()
        {
            var price = _shopData.GetJuicePrice();
            if (_crewManager.Resources.Money < price)
            {
                Debug.LogError("Trying to buy juice without enough money");
                return;
            }

            _crewManager.Resources.AddMoney(-1 * price);
            _crewManager.Resources.AddJuice(10);
        }

        private int GetTransferItemCount()
        {
            return Input.GetKey(KeyCode.LeftShift) ? 5 : 1;
        }
    }
}