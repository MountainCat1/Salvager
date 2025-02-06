using System.Collections.Generic;
using Data;
using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace UI
{
    public class CrewInventoryUI : MonoBehaviour
    {
        [Inject] private ICrewManager _crewManager;
        [Inject] private IDataManager _dataManager;
        
        [SerializeField] private ItemEntryUI itemEntryUIPrefab;
        
        [SerializeField] private Transform creatureInventoryContainer;
        [SerializeField] private Transform crewInventoryContainer;
        
        
        [SerializeField] private TextMeshProUGUI crewNameText;
        
        private CreatureData _selectedCreature;

        private void Start()
        {
            _crewManager.SelectedCreature+= SetSelectedCreature;
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
                _selectedCreature.Inventory.Items.Remove(item);
                _crewManager.Inventory.Items.Add(item);
            }
            else
            {
                _selectedCreature.Inventory.HasDescriptor();
                _crewManager.Inventory.Items.Remove(item);
            }
            
            UpdateCreatureInventory();
            
            _dataManager.SaveData();
        }

        public void SetSelectedCreature(CreatureData creature)
        {
            _selectedCreature = creature;
            
            crewNameText.text = creature.Name;
            
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
                    var itemEntry = Instantiate(itemEntryUIPrefab, creatureInventoryContainer);
                    itemEntry.Set(item, TransferItem);
                }
            }
            
            foreach (Transform child in crewInventoryContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (var item in _crewManager.Inventory.Items)
            {
                var itemEntry = Instantiate(itemEntryUIPrefab, crewInventoryContainer);
                itemEntry.Set(item, TransferItem);
            }
        }
    }
}