using System.Collections.Generic;
using Data;
using Managers;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    [RequireComponent(typeof(UISlide))]
    public class CrewInventoryUI : MonoBehaviour
    {
        [Inject] private ICrewManager _crewManager;
        [Inject] private IDataManager _dataManager;
        [Inject] private DiContainer _diContainer;
        
        [SerializeField] private ItemEntryUI itemEntryUIPrefab;
        
        [SerializeField] private Transform creatureInventoryContainer;
        [SerializeField] private Transform crewInventoryContainer;
        
        
        [SerializeField] private TextMeshProUGUI crewNameText;
        
        private CreatureData _selectedCreature;
        private UISlide  _uiSlide;

        private void Start()
        {
            _crewManager.SelectedCreature+= SetSelectedCreature;
            _crewManager.Changed += UpdateCreatureInventory;
            _uiSlide = GetComponent<UISlide>();
        }

        public void Set(ICollection<CreatureData> crew, InventoryData inventory)
        {
            foreach (var item in inventory.Items)
            {
                var itemEntry = Instantiate(itemEntryUIPrefab, crewInventoryContainer);
                itemEntry.Set(item, TransferItem);
            }
        }

        private void TransferItem(ItemData item)
        {
            if (_selectedCreature == null)
                return;

            if (_selectedCreature.Inventory.Items.Contains(item))
            {
                _selectedCreature.Inventory.RemoveItem(item);
                _crewManager.Inventory.AddItem(item);
            }
            else
            {
                _selectedCreature.Inventory.AddItem(item);
                _crewManager.Inventory.RemoveItem(item);
            }
            
            UpdateCreatureInventory();
            
            _dataManager.SaveData();
        }

        public void SetSelectedCreature(CreatureData creature)
        {
            _selectedCreature = creature;
            
            crewNameText.text = creature.Name;
            
            _uiSlide.ShowPanel();
            
            UpdateCreatureInventory();
        }

        private void UpdateCreatureInventory()
        {
            foreach (Transform child in creatureInventoryContainer)
            {
                Destroy(child.gameObject);
            }

            if (_selectedCreature != null)
            {
                foreach (var item in _selectedCreature.Inventory.Items)
                {
                    var itemEntry = _diContainer.InstantiatePrefab(itemEntryUIPrefab, creatureInventoryContainer);
                    itemEntry.GetComponent<ItemEntryUI>().Set(item, TransferItem);
                }
            }
            
            foreach (Transform child in crewInventoryContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (var item in _crewManager.Inventory.Items)
            {
                var itemEntry = _diContainer.InstantiatePrefab(itemEntryUIPrefab, crewInventoryContainer);
                itemEntry.GetComponent<ItemEntryUI>().Set(item, TransferItem);
            }
        }
    }
}