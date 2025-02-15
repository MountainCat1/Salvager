using Services.MapGenerators;
using UnityEngine;
using Utilities;

namespace LevelSelector.Managers
{
    public interface ILocationGenerator
    {
        LocationData GenerateLocation();
        void AddFeatures(LocationData location);
        void AddEndFeature(LocationData location);
    }
    
    public class LocationGenerator : MonoBehaviour, ILocationGenerator
    {
        [SerializeField] private WeightedLocationFeature originLocationFeatures;
        [SerializeField] private WeightedLocationFeature endLocationFeatures;
        [SerializeField] private WeightedLocationFeature weightedLocationFeatures;
        [SerializeField] private WeightedLocationFeature weightedSecondaryLocationFeatures;
        [SerializeField] private int maxSecondaryFeatures = 3;
        [SerializeField] private int minSecondaryFeatures = 1;
        
        public LocationData GenerateLocation()
        {
            var location = new LocationData()
            {
                Id = System.Guid.NewGuid(),
                MapSettings = GenerateMapSettingsData.FromSettings(GenerateMapSettings.GenerateRandom()),
                Name = Constants.Names.SpaceStations.RandomElement()
            };

            return location;
        }
        
        public void AddFeatures(LocationData location)
        {
            var originFeature = originLocationFeatures.GetRandomItem().ToData();
            location.Features.Add(originFeature);
            
            var mainFeature = weightedLocationFeatures.GetRandomItem().ToData();
            location.Features.Add(mainFeature);
            
            var secondaryFeatures = weightedSecondaryLocationFeatures
                .GetRandomItems(Random.Range(minSecondaryFeatures, maxSecondaryFeatures));

            foreach (var secondaryFeature in secondaryFeatures)
            {
                location.Features.Add(secondaryFeature.ToData());
            }
        }
        
        public void AddEndFeature(LocationData location)
        {
            var endFeature = endLocationFeatures.GetRandomItem().ToData();
            location.Features.Add(endFeature);
        }
    }
    
}