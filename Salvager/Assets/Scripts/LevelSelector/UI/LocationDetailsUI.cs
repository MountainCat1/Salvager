using System.Linq;
using Data;
using LevelSelector.Managers;
using Managers;
using Managers.LevelSelector;
using TMPro;
using UnityEngine;
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

        [SerializeField] private Color shopDescriptionColor;

        private LocationData _selectedLocation;

        private void Start()
        {
            levelSelectorUI.LocationSelected += UpdateDetails;
            _regionManager.RegionChanged += UpdateDetails;
            _crewManager.Changed += UpdateDetails;

            if (_regionManager.Region != null)
            {
                UpdateDetails();
            }
        }

        private void UpdateDetails()
        {
            _selectedLocation = levelSelectorUI.SelectedLocation ??
                                _regionManager.Region.GetLocation(_crewManager.CurrentLocationId);

            if (_selectedLocation == null)
            {
                nameText.text = "No location selected";
                descriptionText.text = "Select a location to view details";
                return;
            }

            Debug.Log($"Selected level: {_selectedLocation.Name}");

            // Update UI
            nameText.text = _selectedLocation.Name;
            descriptionText.text = ConstructDescription(_selectedLocation);

            var locationData = _regionManager.Region.Locations
                .FirstOrDefault(x => x.Id == _selectedLocation.Id);

            if (locationData == null)
            {
                locationData = _regionManager.Region.Locations.First(x => x.Type == LocationType.StartNode);                
            }

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

            if (selectedLocation.ShopData != null)
            {
                string hexColor = ColorUtility.ToHtmlStringRGBA(shopDescriptionColor); 
                description += $"* <color=#{hexColor}>There is a trade ship orbiting this station</color>\n";
            }
            
            description += $"* Detected movement: {selectedLocation.EnemySpawnManaPerSecond:F2}";

            return description;
        }
    }
}