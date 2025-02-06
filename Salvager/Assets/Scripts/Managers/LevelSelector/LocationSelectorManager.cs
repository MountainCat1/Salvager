using System;
using System.Linq;
using Data;
using UnityEngine;
using Zenject;

namespace Managers.LevelSelector
{
    public class LocationSelectorManager : MonoBehaviour
    {
        [Inject] private IRegionGenerator _regionGenerator;
        [Inject] private IRegionManager _regionManager;
        [Inject] private IDataManager _dataManager;
        [Inject] private ICrewManager _crewManager;
        
        [SerializeField] private bool skipLoad = false;
        
        private void Start()
        {
            var data = _dataManager.LoadData();
            
            if(skipLoad || data.Region == null)
            {
                var region = _regionGenerator.Generate();
                _regionManager.SetRegion(region, region.Locations.First(x => x.Type == LevelType.StartNode).Id);

                _crewManager.ReRollCrew();
                
                SaveData();
            }
            
            LoadData();
        }
        private void SaveData()
        {
            var data = new GameData
            {
                Region = RegionData.FromRegion(_regionManager.Region),
                Creatures = _crewManager.Crew.ToList(),
                CurrentLocationId = _regionManager.CurrentLocationId.ToString()
            };
            
            _dataManager.SaveData(data);
        }

        private void LoadData()
        {
            var data = _dataManager.LoadData();
            
            _regionManager.SetRegion(RegionData.ToRegion(data.Region), Guid.Parse(data.CurrentLocationId));
            
            _crewManager.SetCrew(data.Creatures);
        }
    }
}