using System;
using Data;
using UnityEngine;
using Zenject;

namespace Managers.LevelSelector
{
    public interface IRegionManager
    {
        public event Action RegionChanged;
        public Region Region { get; }
    }


    public class RegionManager : MonoBehaviour, IRegionManager
    {
        public event Action RegionChanged;

        [Inject] private IDataManager _dataManager;
        [Inject] private IRegionGenerator _regionGenerator;
        
        [SerializeField] private bool skipLoad = false;

        public Region Region { get; private set; }

        private void Start()
        {
            var data = _dataManager.LoadData();
            
            if (skipLoad || data.Region == null)
            {
                var region = _regionGenerator.Generate();

                var regionData = RegionData.FromRegion(region);
                
                data.Region = regionData;
                
                _dataManager.SaveData(data);
            }

            Region = RegionData.ToRegion(data.Region);
            
            RegionChanged?.Invoke();
        }
    }
}