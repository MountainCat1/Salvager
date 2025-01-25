using System.Linq;
using UnityEngine;
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
            
            var entrance = longestWall[random.Next(longestWall.Count)];

            data.SetTile(entrance.x, entrance.y, TileType.Floor);
            
            Instantiate(entrancePrefab, new Vector3(entrance.x, entrance.y, 0), Quaternion.identity);
        }
    }
}