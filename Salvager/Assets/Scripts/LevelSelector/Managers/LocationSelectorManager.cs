using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Newtonsoft.Json;
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
            
            if(skipLoad || data?.Region == null)
            {
                var region = _regionGenerator.Generate();

                var currentNodeId = region.Locations.First(x => x.Type == LevelType.StartNode).Id;
                _regionManager.SetRegion(region, currentNodeId);

                _crewManager.ReRollCrew();
                
                data = new GameData
                {
                    Region = RegionData.FromRegion(_regionManager.Region),
                    Creatures = _crewManager.Crew.ToList(),
                    Inventory = _crewManager.Inventory,
                    CurrentLocationId = currentNodeId.ToString(),
                    Resources = _crewManager.Resources
                };
            
                _dataManager.SaveData(data);
            }
            
            LoadData();
        }

        private void LoadData()
        {
            var data = _dataManager.LoadData();
            
            _regionManager.SetRegion(RegionData.ToRegion(data.Region), Guid.Parse(data.CurrentLocationId));
            
            _crewManager.SetCrew(data);
        }
    }
}