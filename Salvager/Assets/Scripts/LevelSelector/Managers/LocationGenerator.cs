using Services.MapGenerators;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace LevelSelector.Managers
{
    public interface ILocationGenerator
    {
        Location GenerateLocation();
    }
    
    public class LocationGenerator : MonoBehaviour, ILocationGenerator
    {
        [SerializeField] private WeightedLocationFeature weightedLocationFeatures;
        
        public Location GenerateLocation()
        {
            var location = new Location()
            {
                Settings = GenerateMapSettings.GenerateRandom(),
                Name = Constants.Names.SpaceStations.RandomElement()
            };
            
            var mainFeature = weightedLocationFeatures.GetRandomItem().ToData();
            
            location.Features.Add(mainFeature);

            return location;
        }
    }
}