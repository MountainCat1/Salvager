using System;
using UI.Abstractions;
using UnityEngine;
using Zenject;

namespace UI
{
    public interface IPopupManagerUI
    {
        public InventoryUI ShowInventory(Creature creature);
    }

    public class PopupManagerUI : MonoBehaviour, IPopupManagerUI
    {
        [SerializeField] private InventoryUI inventoryUIPrefab;
        [SerializeField] private Transform popupParent;

        private InventoryUI _instantiatedInventoryUI;

        [Inject] private DiContainer _container;

        public InventoryUI ShowInventory(Creature creature)
        {
            if (_instantiatedInventoryUI)
                _instantiatedInventoryUI = InstantiatePopup<InventoryUI>(inventoryUIPrefab);

            _instantiatedInventoryUI.SetCreature(creature);
            _instantiatedInventoryUI.Show();

            return _instantiatedInventoryUI;
        }
        
        private T InstantiatePopup<T>(T prefab) where T : PopupUI
        {
            return _container.InstantiatePrefabForComponent<T>(prefab, popupParent);
        }
    }
}