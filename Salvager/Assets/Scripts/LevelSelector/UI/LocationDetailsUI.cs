using System.Linq;
using Data;
using LevelSelector.Managers;
using Managers;
using Managers.LevelSelector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;
using Zenject;

namespace UI
{
    public class LocationDetailsUI : MonoBehaviour
    {
        [Inject] private IRegionManager _regionManager;
        [Inject] private IDataManager _dataManager;
        [Inject] private ICrewManager _crewManager;
        [Inject] private ILocationInteractionManager _locationInteractionManager;

        [SerializeField] private LevelSelectorUI levelSelectorUI;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        [SerializeField] private SceneReference levelScene;

        [SerializeField] private Transform locationInteractionsParent;
        [SerializeField] private Button buttonPrefab;

        private LocationData _selectedLocation;

        private void Start()
        {
            levelSelectorUI.LocationSelected += UpdateDetails;
            _regionManager.RegionChanged += UpdateDetails;
            _crewManager.Changed += UpdateDetails;

            _selectedLocation = _regionManager.Region.Locations.First(l => l.Id == _crewManager.CurrentLocationId);
            
            if (_regionManager.Region != null)
            {
                UpdateDetails();
            }
        }

        private void UpdateDetails()
        {
            _selectedLocation = levelSelectorUI.SelectedLocation ??
                                _regionManager.Region.GetLocation(_crewManager.CurrentLocationId);

            Debug.Log($"Selected level: {_selectedLocation.Name}");

            // Update UI
            nameText.text = _selectedLocation.Name;
            descriptionText.text = ConstructDescription(_selectedLocation);

            var locationData = _dataManager
                .GetData().Region.Locations
                .First(x => x.Id == _selectedLocation.Id);

            foreach (Transform child in locationInteractionsParent)
            {
                Destroy(child.gameObject);
            }

            foreach (var locationInteraction in _locationInteractionManager.Interactions)
            {
                if (!locationInteraction.IsDisplayed(locationData))
                    continue;

                var button = Instantiate(buttonPrefab, locationInteractionsParent);
                button.onClick.AddListener(() => locationInteraction.OnClick(locationData));

                button.GetComponentInChildren<TextMeshProUGUI>().text = locationInteraction.Message;
                button.interactable = locationInteraction.IsEnabled(locationData);
            }
        }

        private string ConstructDescription(LocationData selectedLocation)
        {
            var description = string.Empty;

            foreach (var feature in selectedLocation.Features)
            {
                description += $"* {feature.Description}\n";
            }

            return description;
        }
    }
}