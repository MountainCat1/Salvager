using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace UI
{
    public interface IPanelManagerUI
    {
        void Toggle(PanelManagerUI.UIPanel panel);
        void Show(PanelManagerUI.UIPanel panel);
        void ClearPanels();
    }
    
    public class PanelManagerUI : MonoBehaviour, IPanelManagerUI
    {
        [SerializeField] private UISlide inventoryPanel;
        [SerializeField] private UISlide shopPanel;
        [SerializeField] private UISlide crewPanel;
        [SerializeField] private UISlide travelPanel;
        [SerializeField] private UISlide upgradePanel;
        
        private Dictionary<UIPanel, UISlide> _panels;
        
        [Inject] private IInputManager _inputManager;

        private void Start()
        {
            _panels = new Dictionary<UIPanel, UISlide>
            {
                {UIPanel.Inventory, inventoryPanel},
                {UIPanel.Shop, shopPanel},
                {UIPanel.Travel, travelPanel},
                {UIPanel.Upgrade, upgradePanel}
            };
            
            if(_panels.Any(x => x.Value == null))
                Debug.LogWarning($"Some panels are not assigned. {string.Join(", ", _panels.Where(x => x.Value == null).Select(x => x.Key))}");;
            
            _inputManager.UI.GoBack += OnGoBack;
        }

        private void OnGoBack()
        {
            ClearPanels();
        }

        public enum UIPanel
        {
            Inventory,
            Shop,
            Travel,
            Upgrade
        }

        public void Toggle(UIPanel panel)
        {
            foreach (var (panelEnum, uiElement) in _panels)
            {
                if (panelEnum == panel)
                {
                    uiElement.TogglePanel();
                }
                else
                {
                    uiElement?.HidePanel();
                }
            }
        }

        public void Show(UIPanel panel)
        {
            foreach (var (panelEnum, uiElement) in _panels)
            {
                if (panelEnum == panel)
                {
                    uiElement.ShowPanel();
                }
                else
                {
                    uiElement?.HidePanel();
                }
            }
        }

        public void ClearPanels()
        {
            foreach (var (_, uiElement) in _panels)
            {
                uiElement?.HidePanel();
            }
        }
    }
}