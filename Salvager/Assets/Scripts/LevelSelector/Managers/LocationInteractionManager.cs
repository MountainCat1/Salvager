using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Managers;
using Managers.LevelSelector;
using Services.MapGenerators;
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
        
        [SerializeField] private SceneReference levelScene;
        
        public IReadOnlyList<LocationInteraction> Interactions => _interactions;
        
        private readonly List<LocationInteraction> _interactions = new();
        
        private void Awake()
        {
            _interactions.Add(new LocationInteraction
            {
                Message = "Inventory",
                IsDisplayed = _ => true,
                IsEnabled = _ => true,
                OnClick = _ =>
                {
                    inventorySlide.TogglePanel();
                }
            });   
            
            _interactions.Add(new LocationInteraction
            {
                Message = "Embark",
                IsDisplayed = location => _regionManager.GetDistance(Guid.Parse(location.Id), _regionManager.CurrentLocationId) == 1,
                IsEnabled = _ => true,
                OnClick = location =>
                {
                    _regionManager.ChangeCurrentLocation(LocationData.ToLocation(location));
                    SaveData();
                }
            });
            
            _interactions.Add(new LocationInteraction
            {
                Message = "Land",
                IsDisplayed = location => _regionManager.CurrentLocationId.ToString() == location.Id,
                IsEnabled = location => !location.Visited,
                OnClick = LoadLevel
            });
            
            _interactions.Add(new LocationInteraction
            {
                Message = "Probe",
                IsDisplayed = location => _regionManager.CurrentLocationId.ToString() == location.Id,
                IsEnabled = _ => false,
                OnClick = _ => throw new NotImplementedException()
            });
        }

        private void SaveData()
        {
            var gameData = _dataManager.GetData();
            
            gameData.Inventory = _crewManager.Inventory;
            gameData.Creatures = _crewManager.Crew.ToList();
            gameData.CurrentLocationId = _regionManager.CurrentLocationId.ToString();
            gameData.Region = RegionData.FromRegion(_regionManager.Region);
            
            _dataManager.SaveData(gameData);
        }

        public void LoadLevel(LocationData locationData)
        {
            Debug.Log($"Loading level: {locationData.Name}");
            
            locationData.Visited = true;
            _regionManager.Region.Locations.First(l => l.Id == Guid.Parse(locationData.Id)).Visited = true;
            
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
    }
}