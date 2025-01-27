using System.Linq;
using Managers;
using UI.Abstractions;
using UnityEngine;
using Zenject;

namespace UI
{
    public interface IInventoryDisplayUI
    {
        public InventoryUI ShowInventory(Creature creature);
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
            
            instantiatedInventoryUI.Hide();
        }

        private void OnSelectionChanged()
        {
            if(!instantiatedInventoryUI.IsVisible)
                return;
            
            if(_selectionManager.SelectedCreatures.Contains(instantiatedInventoryUI.Creature))
                return;

            if (!_selectionManager.SelectedCreatures.Any())
            {
                instantiatedInventoryUI.Hide();
                return;
            }
            
            ShowInventory(_selectionManager.SelectedCreatures.First());
        }

        private void OnInventoryPressed()
        {
            if(instantiatedInventoryUI.IsVisible)
            {
                instantiatedInventoryUI.Hide();
                return;
            }
            
            if(!_selectionManager.SelectedCreatures.Any())
                return;
            
            var creature = _selectionManager.SelectedCreatures.First();

            ShowInventory(creature);
        }

        public InventoryUI ShowInventory(Creature creature)
        {
            instantiatedInventoryUI.SetCreature(creature);
            instantiatedInventoryUI.Show();

            return instantiatedInventoryUI;
        }
        
        private T InstantiatePopup<T>(T prefab) where T : PopupUI
        {
            return _container.InstantiatePrefabForComponent<T>(prefab, popupParent);
        }
    }
}