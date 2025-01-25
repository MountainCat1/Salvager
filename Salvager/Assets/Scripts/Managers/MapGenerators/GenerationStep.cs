using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Services.MapGenerators
{
    public enum MapLayer
    {
        Wall,
        Floor,
        Shadow
    }
    
    public class GenerateMapData
    {
        public Dictionary<MapLayer, List<Vector2Int>> MapTiles { get; }

        public GenerateMapData()
        {
            MapTiles = new Dictionary<MapLayer, List<Vector2Int>>();
            
            foreach (var mapLayer in Enum.GetValues(typeof(MapLayer)).Cast<MapLayer>())
            {
                MapTiles[mapLayer] = new List<Vector2Int>();
            }
        }
    }
    
    public abstract class GenerationStep : MonoBehaviour
    {
        public void Generate(GenerateMapData data)
        {
            // Generate the map
        }
    }
}