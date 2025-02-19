using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace UI.Abstractions
{
    public class PageManager : MonoBehaviour
    {
        [Inject] private IInputManager _inputManager;
        
        [SerializeField] private PageUI initialPage;
        
        private PageUI[] _pages;
        
        private Stack<PageUI> _pageHistory = new Stack<PageUI>();

        private PageUI _currentPage;
        
        private void Awake()
        {
            // Get all children of this object that are PageUI
            _pages = GetComponentsInChildren<PageUI>(true);
            
            // Hide all pages
            foreach (var page in _pages)
            {
                page.Hide();
            }
            
            // Show the initial page
            ShowPage(initialPage);
        }

        private void Start()
        {
            _inputManager.UI.GoBack += GoBack;
        }

        private void OnDestroy()
        {
            _inputManager.UI.GoBack -= GoBack;
        }

        public void ShowPage(PageUI page)
        {
            // Hide all pages
            foreach (var p in _pages)
            {
                p.Hide();
            }
            
            if(_currentPage)
                _pageHistory.Push(_currentPage);
            
            // Show the selected page
            page.Show();
            _currentPage = page;
        }
        
        public void GoBack()
        {
            if (_pageHistory.Count == 0)
            {
                ShowPage(initialPage);
                return;
            }
            
            _currentPage.Hide();
            ShowPage(_pageHistory.Pop());
        }
    }
}