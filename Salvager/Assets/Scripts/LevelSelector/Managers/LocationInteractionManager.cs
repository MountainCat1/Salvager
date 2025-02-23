using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Managers;
using Managers.LevelSelector;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using Zenject;

namespace LevelSelector.Managers
{
    public class LocationInteraction
    {
        public Func<LocationData, bool> IsDisplayed;
        public Func<LocationData, bool> IsEnabled;
        public string Message;
        public Action<LocationData> OnClick;
    }
    
    public interface ILocationInteractionManager
    {
        IReadOnlyList<LocationInteraction> Interactions { get; }
    }
    
    public class LocationInteractionManager : MonoBehaviour, ILocationInteractionManager
    {
        [Inject] IRegionManager _regionManager;
        [Inject] IDataManager _dataManager;
        [Inject] IDataResolver _dataResolver;
        [Inject] ICrewManager _crewManager;
        
        [SerializeField] private UISlide inventorySlide;
        [SerializeField] private UISlide shopSlide;
        [SerializeField] private UISlide upgradeSlide;
        
        [SerializeField] private SceneReference levelScene;
        
        public IReadOnlyList<LocationInteraction> Interactions => _interactions;
        
        private readonly List<LocationInteraction> _interactions = new();
        
        private void Awake()
        {
            _interactions.Add(new LocationInteraction
            {
                Message = "Embark",
                IsDisplayed = location => _regionManager.GetDistance(location.Id, _crewManager.CurrentLocationId) == 1,
                IsEnabled = _ => _crewManager.CanTravel(),
                OnClick = location =>
                {
                    _crewManager.ChangeCurrentLocation(location);
                    SaveData();
                }
            });
            
            _interactions.Add(new LocationInteraction
            {
                Message = "Land",
                IsDisplayed = location => _crewManager.CurrentLocationId == location.Id,
                IsEnabled = location => !location.Visited && _crewManager.Crew.Any(x => x.Selected),
                OnClick = LoadLevel
            });
            
            _interactions.Add(new LocationInteraction
            {
                Message = "Probe",
                IsDisplayed = IsCurrentLocation,
                IsEnabled = _ => false,
                OnClick = _ => throw new NotImplementedException()
            });
            
            _interactions.Add(new LocationInteraction
            {
                Message = "Inventory",
                IsDisplayed = _ => true,
                IsEnabled = _ => true,
                OnClick = _ =>
                {
                    shopSlide.HidePanel();
                    upgradeSlide.HidePanel();
                    
                    inventorySlide.TogglePanel();
                }
            });   
            
            _interactions.Add(new LocationInteraction
            {
                Message = "Upgrade",
                IsDisplayed = _ => true,
                IsEnabled = _ => true,
                OnClick = location =>
                {
                    inventorySlide.HidePanel();
                    shopSlide.HidePanel();
                    
                    upgradeSlide.TogglePanel();
                }
            });
            
            _interactions.Add(new LocationInteraction
            {
                Message = "Trade",
                IsDisplayed = location => IsCurrentLocation(location) && location.ShopData is not null,
                IsEnabled = _ => true,
                OnClick = _ =>
                {
                    inventorySlide.HidePanel();
                    upgradeSlide.HidePanel();
                    
                    shopSlide.TogglePanel();
                }
            }); 
        }

        private void SaveData()
        {
            var gameData = _dataManager.GetData();
            
            gameData.Inventory = _crewManager.Inventory;
            gameData.Creatures = _crewManager.Crew.ToList();
            gameData.CurrentLocationId = _crewManager.CurrentLocationId.ToString();
            gameData.Region = RegionData.FromRegion(_regionManager.Region);
            
            _dataManager.SaveData();
        }

        public void LoadLevel(LocationData locationData)
        {
            Debug.Log($"Loading level: {locationData.Name}");
            
            locationData.Visited = true;
            _regionManager.Region.Locations.First(l => l.Id == locationData.Id).Visited = true;
            
            SaveData();
            
            GameManager.GameSettings = new GameSettings
            {
                Settings = GenerateMapSettingsData.ToSettings(locationData.MapSettings),
                Name = locationData.Name,
                Location = locationData
            };
            
            // Load level scene
            SceneManager.LoadScene(levelScene);
        }
        
        private bool IsCurrentLocation(LocationData locationData)
        {
            return _crewManager.CurrentLocationId == locationData.Id;
        }
    }
}