using System.Linq;
using Managers;
using UI.Abstractions;
using UnityEngine;
using Zenject;

namespace UI
{
    public interface IInventoryDisplayUI
    {
        void ShowInventory(Creature creature);
    }

    public class InventoryDisplayUI : MonoBehaviour, IInventoryDisplayUI
    {
        [Inject] private IInputManager _inputManager;
        [Inject] private ISelectionManager _selectionManager;
        
        [SerializeField] private Transform popupParent;
        [SerializeField] private InventoryUI instantiatedInventoryUI;

        [Inject] private DiContainer _container;

        void Start()
        {
            _inputManager.UI.ShowInventory += OnInventoryPressed;
            _selectionManager.OnSelectionChanged += OnSelectionChanged;
            
            instantiatedInventoryUI.Toggle(false);
        }

        private void OnDestroy()
        {
            _inputManager.UI.ShowInventory -= OnInventoryPressed;
            _selectionManager.OnSelectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            instantiatedInventoryUI.SetCreature(_selectionManager.SelectedCreatures.FirstOrDefault());
        }

        private void OnInventoryPressed()
        {
            ToggleInventory();
        }

        public InventoryUI ToggleInventory()
        {
            instantiatedInventoryUI.Toggle();

            return instantiatedInventoryUI;
        }

        public void ShowInventory(Creature creature)
        {
            instantiatedInventoryUI.SetCreature(creature);
            instantiatedInventoryUI.Toggle(true);
        }
    }
}