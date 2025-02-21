using System;
using LevelSelector.Managers;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class UpgradeMenuUI : MonoBehaviour
    {
        [Inject] private IItemManager _itemManager;
        [Inject] private IItemDescriptionManager _itemDescriptionManager;
        [Inject] private ICrewManager _crewManager;
        [Inject] private IUpgradeManager _upgradeManager;
        [Inject] private DiContainer _diContainer;

        [SerializeField] private TextMeshProUGUI selectedItemName;
        [SerializeField] private TextMeshProUGUI selectedItemDescription;
        [SerializeField] private Image selectedItemIcon;

        [SerializeField] private Transform playerInventoryContainer;
        [SerializeField] private ItemEntryUI itemEntryUIPrefab;

        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button reforgeButton;
        [SerializeField] private Button scrapButton;

        private ItemData _selectedItem;

        private void Start()
        {
            _crewManager.Changed += UpdateUI;
            if (_crewManager.Inventory is not null)
                UpdateUI();

            upgradeButton.onClick.AddListener(UpgradeItem);
            reforgeButton.onClick.AddListener(ReforgeItem);
            scrapButton.onClick.AddListener(ScrapItem);
        }

        private void ScrapItem()
        {
            throw new NotImplementedException();
        }

        private void ReforgeItem()
        {
            throw new NotImplementedException();
        }

        private void UpgradeItem()
        {
            _upgradeManager.UpgradeItem(_selectedItem);
        }

        private void UpdateUI()
        {
            SelectItem(null);

            foreach (Transform child in playerInventoryContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (var item in _crewManager.Inventory.Items)
            {
                var itemEntry = _diContainer
                    .InstantiatePrefab(itemEntryUIPrefab, playerInventoryContainer)
                    .GetComponent<ItemEntryUI>();

                itemEntry.Set(item, SelectItem);
            }
        }

        private void SelectItem(ItemData itemData)
        {
            if (itemData == null)
            {
                selectedItemName.text = string.Empty;
                selectedItemDescription.text = string.Empty;
                selectedItemIcon.sprite = null;
                _selectedItem = null;
                return;
            }

            selectedItemName.text = itemData.Prefab.Name;
            selectedItemDescription.text = _itemDescriptionManager.GetDescription(itemData);
            selectedItemIcon.sprite = itemData.Prefab.Icon;

            _selectedItem = itemData;
        }
    }
}