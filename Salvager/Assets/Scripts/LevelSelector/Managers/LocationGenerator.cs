using Services.MapGenerators;
using UnityEngine;
using Utilities;

namespace LevelSelector.Managers
{
    public interface ILocationGenerator
    {
        Location GenerateLocation();
    }
    
    public class LocationGenerator : MonoBehaviour, ILocationGenerator
    {
        [SerializeField] private WeightedLocationFeature originLocationFeatures;
        [SerializeField] private WeightedLocationFeature weightedLocationFeatures;
        [SerializeField] private WeightedLocationFeature weightedSecondaryLocationFeatures;
        [SerializeField] private int maxSecondaryFeatures = 3;
        [SerializeField] private int minSecondaryFeatures = 1;
        
        public Location GenerateLocation()
        {
            var location = new Location()
            {
                Settings = GenerateMapSettings.GenerateRandom(),
                Name = Constants.Names.SpaceStations.RandomElement()
            };
            
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

            return location;
        }
    }
}