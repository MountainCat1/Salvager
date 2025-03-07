using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI
{
    public class InventoryUI : MonoBehaviour, IPointerDownHandler
    {
        [Inject] private IInputManager _inputManager;
        [Inject] private DiContainer _diContainer;

        [SerializeField] private InventoryEntryUI inventoryEntryUIPrefab;
        [SerializeField] private Transform entriesParent;
        [SerializeField] private TextMeshProUGUI creatureNameText;

        private Creature _creature;
        public bool IsToggled { get; private set; }

        public Creature Creature => _creature;

        public void Toggle()
        {
            IsToggled = !IsToggled;
            UpdateInventory();
        }
        
        public void Toggle(bool toggle)
        {
            IsToggled = toggle;
            UpdateInventory();
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetCreature(Creature creature)
        {
            _creature = creature;
            
            if (creature == null)
            {
                Hide();
                return;
            }
            
            _creature.Inventory.Changed += UpdateInventory;
            UpdateInventory();
        }

        private void UpdateInventory()
        {
            if (!IsToggled || _creature == null)
                Hide();
            else
                Show();
            
            foreach (Transform child in entriesParent)
            {
                Destroy(child.gameObject);
            }

            creatureNameText.text = _creature.name;

            foreach (var item in _creature.Inventory.Items)
            {
                var inventoryEntryUI = _diContainer.InstantiatePrefab(inventoryEntryUIPrefab, entriesParent);
                inventoryEntryUI.GetComponent<InventoryEntryUI>().SetItem(item, _creature);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.SetAsLastSibling();
        }
    }
}

