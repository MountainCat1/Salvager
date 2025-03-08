using Services.MapGenerators;
using UnityEngine;
using Utilities;

namespace LevelSelector.Managers
{
    public interface ILocationGenerator
    {
        LocationData GenerateLocation(System.Random random);
        void AddFeatures(LocationData location, RegionType regionType);
        void AddEndFeature(LocationData location, RegionType regionType);
    }
    
    public class LocationGenerator : MonoBehaviour, ILocationGenerator
    {
        public LocationData GenerateLocation(System.Random random)
        {
            var location = new LocationData()
            {
                Id = System.Guid.NewGuid(),
                MapSettings = GenerateMapSettingsData.FromSettings(GenerateMapSettings.GenerateRandom(random)),
                Name = Constants.Names.SpaceStations.RandomElement(),
                EnemySpawnManaPerSecond = Random.Range(0.5f, 3f),
            };

            return location;
        }
        
        public void AddFeatures(LocationData location, RegionType regionType)
        {
            var originFeature = regionType.originLocationFeatures.GetRandomItem().ToData();
            location.Features.Add(originFeature);
            
            var mainFeature = regionType.weightedLocationFeatures.GetRandomItem().ToData();
            location.Features.Add(mainFeature);
            
            var secondaryFeatures = regionType.weightedSecondaryLocationFeatures
                .GetRandomItems(Random.Range(regionType.minSecondaryFeatures, regionType.maxSecondaryFeatures));

            foreach (var secondaryFeature in secondaryFeatures)
            {
                location.Features.Add(secondaryFeature.ToData());
            }
        }
        
        public void AddEndFeature(LocationData location, RegionType regionType)
        {
            var endFeature = regionType.endLocationFeatures.GetRandomItem().ToData();
            location.Features.Add(endFeature);
        }
    }
    
}