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
    [RequireComponent(typeof(UISlide))]
    public class UpgradeMenuUI : MonoBehaviour
    {
        [Inject] private IItemManager _itemManager;
        [Inject] private IItemDescriptionManager _itemDescriptionManager;
        [Inject] private ICrewManager _crewManager;
        [Inject] private IUpgradeManager _upgradeManager;
        [Inject] private DiContainer _diContainer;
        [Inject] private ISoundPlayer _soundPlayer;
        [Inject] private IFloatingTextService _floatingTextService;

        [SerializeField] private TextMeshProUGUI selectedItemName;
        [SerializeField] private TextMeshProUGUI selectedItemDescription;
        [SerializeField] private Image selectedItemIcon;

        [SerializeField] private Transform playerInventoryContainer;
        [SerializeField] private ItemEntryUI itemEntryUIPrefab;

        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button reforgeButton;
        [SerializeField] private Button scrapButton;
        
        [SerializeField] private TextMeshProUGUI scrapValueText;
        [SerializeField] private TextMeshProUGUI upgradeCostText;
        
        [SerializeField] private AudioClip badUpgradeSound;
        [SerializeField] private AudioClip goodUpgradeSound;
        [SerializeField] private AudioClip normalUpgradeSound;
        
        [SerializeField] private Transform floatingTextParent;

        private ItemData _selectedItem;

        private UISlide _uiSlide;

        private void Start()
        {
            _crewManager.Changed += UpdateUI;
            if (_crewManager.Inventory is not null)
                UpdateUI();

            upgradeButton.onClick.AddListener(UpgradeItem);
            reforgeButton.onClick.AddListener(ReforgeItem);
            scrapButton.onClick.AddListener(ScrapItem);

            _uiSlide = GetComponent<UISlide>();
            _uiSlide.Showed += UpdateUI;
        }

        private void ScrapItem()
        {
            _upgradeManager.ScrapItem(_selectedItem);

            UpdateUI();
        }

        private void ReforgeItem()
        {
            // TODO: Implement

            UpdateUI();
        }

        private void UpgradeItem()
        {
            var upgradeResult = _upgradeManager.PlayerBuyUpgrade(_selectedItem);
            
            switch (upgradeResult)
            {
                case UpgradeResult.Bad:
                    _soundPlayer.PlaySoundGlobal(badUpgradeSound, SoundType.UI);
                    _floatingTextService.Show("Upgrade failed", floatingTextParent.transform.position, Color.red, 1.5f);;
                    break;
                case UpgradeResult.Good:
                    _soundPlayer.PlaySoundGlobal(goodUpgradeSound, SoundType.UI);
                    _floatingTextService.Show("Upgrade successful", floatingTextParent.transform.position, Color.green, 1.5f);
                    break;
                case UpgradeResult.Normal:
                    _soundPlayer.PlaySoundGlobal(normalUpgradeSound, SoundType.UI);
                    _floatingTextService.Show("Upgrade successful", floatingTextParent.transform.position, Color.yellow, 1.5f);
                    break;
            }
            
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_crewManager.Inventory.ContainsItem(_selectedItem) == false)
                SelectItem(null);
            else
                SelectItem(_selectedItem);

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
                
                upgradeCostText.text = "0";
                scrapValueText.text = "0";
                
                reforgeButton.interactable = false;
                upgradeButton.interactable = false;
                scrapButton.interactable = false;
                
                return;
            }

            selectedItemName.text = itemData.Prefab.Name;
            selectedItemDescription.text = _itemDescriptionManager.GetDescription(itemData);
            selectedItemIcon.sprite = itemData.Prefab.Icon;
            _selectedItem = itemData;
            
            upgradeCostText.text = $"{_upgradeManager.GetUpgradeCost(_selectedItem)}";
            scrapValueText.text = $"{_upgradeManager.GetScrapValue(_selectedItem)}";
            
            reforgeButton.interactable = false;
            upgradeButton.interactable = _upgradeManager.CanUpgrade(_selectedItem);
            scrapButton.interactable = _upgradeManager.CanScrap(_selectedItem);
        }
    }
}