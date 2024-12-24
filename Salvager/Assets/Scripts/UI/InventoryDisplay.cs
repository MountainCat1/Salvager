using Managers;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    public class InventoryDisplay : MonoBehaviour
    {
        [Inject] IPlayerCharacterProvider _playerCharacterProvider;
        [Inject] private DiContainer _container;

        [SerializeField] private InventoryEntryDisplay inventoryEntryDisplayPrefab;
        [SerializeField] private Transform inventoryParent;
        
        [SerializeField] private TextMeshProUGUI currentWeaponText;
        
        private Creature _creature;

        private void Start()
        {
            _creature = _playerCharacterProvider.Get();
            _creature.Inventory.OnChange += UpdateInventory;
            _creature.WeaponChanged += UpdateWeapon;
            UpdateInventory();
        }

        private void UpdateWeapon()
        {
            if (!_creature.Weapon)
            {
                currentWeaponText.text = "No Weapon";
                return;
            }
            
            currentWeaponText.text = _creature.Weapon.Name;
        }

        private void UpdateInventory()
        {
            foreach (Transform child in inventoryParent)
            {
                Destroy(child.gameObject);
            }

            foreach (var item in _creature.Inventory.Items)
            {
                var entry = Instantiate(inventoryEntryDisplayPrefab, inventoryParent);
                _container.Inject(entry);
                entry.Initialize(item);
            }
        }
    }
}