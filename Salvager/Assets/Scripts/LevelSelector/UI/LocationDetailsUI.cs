using Managers;
using Managers.LevelSelector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        [SerializeField] private GameObject currentLocationUI;
        [SerializeField] private GameObject notCurrentLocationUI; 
        
        private Location _selectedLocation;

        private void Start()
        {
            levelSelectorUI.LocationSelected += UpdateDetails;
            _regionManager.RegionChanged += UpdateDetails;

            if (_regionManager.Region != null)
            {
                _selectedLocation = _regionManager.CurrentLocation;
                UpdateDetails();
            }
        }

        private void UpdateDetails()
        {
            _selectedLocation = levelSelectorUI.SelectedLocation ?? _regionManager.CurrentLocation;
            
            Debug.Log($"Selected level: {_selectedLocation.Name}");
            
            // Update UI
            nameText.text = _selectedLocation.Name;
            descriptionText.text = ConstructDescription(_selectedLocation);
            
            if(_selectedLocation.DistanceToCurrent == 0)
            {
                currentLocationUI.SetActive(true);
                notCurrentLocationUI.SetActive(false);
            }
            else if(_selectedLocation.DistanceToCurrent == 1)
            {
                currentLocationUI.SetActive(false);
                notCurrentLocationUI.SetActive(true);
            }
            else
            {
                currentLocationUI.SetActive(false);
                notCurrentLocationUI.SetActive(false);
            }
        }

        private string ConstructDescription(Location selectedLocation)
        {
            var description = string.Empty;

            foreach (var feature in selectedLocation.Features)
            {
                description += $"* {feature.Description}\n";
            }
            
            return description;
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
                Name = _selectedLocation.Name,
                Location = LocationData.FromLocation(_selectedLocation)
            };
            
            // Load level scene
            SceneManager.LoadScene(levelScene);
        }
    }
}