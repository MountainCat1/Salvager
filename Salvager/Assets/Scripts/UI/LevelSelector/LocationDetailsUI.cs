using System;
using Managers;
using Managers.LevelSelector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Utilities;
using Zenject;

namespace UI
{
    public class LocationDetailsUI : MonoBehaviour
    {
        [Inject] private IRegionManager _regionManager;
        
        [SerializeField] private LevelSelectorUI levelSelectorUI;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        
        [SerializeField] private SceneReference levelScene;
        
        private Location _selectedLocation;

        private void Start()
        {
            levelSelectorUI.LocationSelected += OnLevelSelected;
        }

        private void OnLevelSelected(Location location)
        {
            _selectedLocation = location;
            
            Debug.Log($"Selected level: {location.Name}");
            
            // Update UI
            nameText.text = location.Name;
            descriptionText.text = $"Room count: {location.Settings.roomCount}\n" +
                                                $"Room size: {location.Settings.roomMaxSize}\n" +
                                                $"Room count: {location.Settings.roomCount}";
        }

        // Button Callbacks
        public void Embark()
        {
            if (_selectedLocation == null)
            {
                Debug.LogError("No level selected!");
                return;
            }
            
            Debug.Log($"Embarking on level: {_selectedLocation.Name}");
            
            _regionManager.ChangeCurrentLocation(_selectedLocation);
        }

        public void LoadLevel()
        {
            if (_selectedLocation == null)
            {
                Debug.LogError("No level selected!");
                return;
            }
            
            Debug.Log($"Loading level: {_selectedLocation.Name}");
            
            GameManager.GameSettings = new GameSettings
            {
                Settings = _selectedLocation.Settings,
                RoomBlueprints = _selectedLocation.RoomBlueprints,
                Name = _selectedLocation.Name
            };
            
            // Load level scene
            SceneManager.LoadScene(levelScene);
        }
    }
}