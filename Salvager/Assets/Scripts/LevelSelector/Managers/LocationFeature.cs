using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine;

namespace LevelSelector.Managers
{
    [Serializable]
    public class FeatureEnemies
    {
        public Creature Creature;
    }
    
    [CreateAssetMenu(fileName = "LocationFeature", menuName = "Custom/LocationFeature", order = 0)]
    public class LocationFeature : ScriptableObject {
        public string description = string.Empty;
        public List<FeatureEnemies> enemies = new();
        public List<RoomBlueprint> roomBlueprints = new();
        public List<RoomBlueprint> genericRoomBlueprints = new();
        
        public LocationFeatureData ToData()
        {
            return new LocationFeatureData
            {
                Description = description,
                Enemies = enemies.Select(LocationFeatureData.FeatureEnemyData.FromFeatureEnemy).ToList(),
                Name = name,
                RoomBlueprints = roomBlueprints.Select(x => x.name).ToArray(),
                GenericRoomBlueprints = genericRoomBlueprints.Select(x => x.name).ToArray(),
            };
        }
    }
}