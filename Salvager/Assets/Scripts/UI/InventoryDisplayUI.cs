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
        [Inject] private DiContainer _container;

        [SerializeField] private Transform popupParent;
        [SerializeField] private InventoryUI instantiatedInventoryUI;

        private void Start()
        {
            _inputManager.UI.ShowInventory += ToggleInventory;
            _selectionManager.OnSelectionChanged += UpdateSelectedCreature;

            instantiatedInventoryUI.Toggle(false);
        }

        private void OnDestroy()
        {
            _inputManager.UI.ShowInventory -= ToggleInventory;
            _selectionManager.OnSelectionChanged -= UpdateSelectedCreature;
        }

        private void UpdateSelectedCreature()
        {
            instantiatedInventoryUI.SetCreature(_selectionManager.SelectedCreatures.FirstOrDefault());
        }

        private void ToggleInventory()
        {
            instantiatedInventoryUI.Toggle();
        }

        public void ShowInventory(Creature creature)
        {
            instantiatedInventoryUI.SetCreature(creature);
            instantiatedInventoryUI.Toggle(true);
        }
    }
}