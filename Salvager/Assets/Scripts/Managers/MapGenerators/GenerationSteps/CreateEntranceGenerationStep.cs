using System.Linq;
using Constants;
using UnityEngine;
using Utilities;
using Random = System.Random;

namespace Services.MapGenerators.GenerationSteps
{
    public class CreateEntranceGenerationStep : GenerationStep
    {
        [SerializeField] private GameObject entrancePrefab = null!;
        
        public override void Generate(GenerateMapData data, GenerateMapSettings settings, Random random)
        {
            var wallTiles = data.CreateTileList(TileType.Wall);
            var wallClusters = TileCluster.GetConnectedClusters(wallTiles);
            var longestWall = wallClusters.OrderByDescending(x => x.Count).First();

            var validEntrancePoints = longestWall.Where(v => ValidEntrancePoint(v, data));
            var entrance = validEntrancePoints.RandomElement();

            data.SetTile(entrance.x, entrance.y, TileType.Floor);
            
            Instantiate(entrancePrefab, new Vector3(entrance.x, entrance.y, 0), Quaternion.identity);
        }

        private bool ValidEntrancePoint(Vector2Int v, GenerateMapData data)
        {
            var directions = DirectionsInt.Directions;
            
            int floorCount = 0;
            int wallCount = 0;

            foreach (var direction in directions)
            {
                var x = v.x + direction.x;
                var y = v.y + direction.y;
                
                if(!data.IsInBounds(x, y))
                {
                    return false;
                }

                if (data.GetTile(x, y) == (int) TileType.Floor)
                {
                    floorCount++;
                }
                else if (data.GetTile(x, y) == (int) TileType.Wall)
                {
                    wallCount++;
                }
            }
            
            if (floorCount == 1 && wallCount == 2)
            {
                return true;
            }

            return false;
        }
    }
}