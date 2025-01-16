using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;
using Zenject;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [Inject] private IMapGenerator _mapGenerator;
        [Inject] private DiContainer _container;
        [Inject] private ISpawnerManager _spawnerManager;
        [Inject] private ICameraController _cameraController;


        [SerializeField] private Creature playerPrefab;
        [SerializeField] private Creature enemyPrefab;

        private void Start()
        {
            _mapGenerator.GenerateMap();

            var map = _mapGenerator.MapData;
            
            var startingRoom = map.GetAllRooms().First(x => x.IsEntrance);
            
            var firstUnit = _spawnerManager.SpawnCreature(playerPrefab, (Vector2)startingRoom.Positions.RandomElement() * map.TileSize);
            _spawnerManager.SpawnCreature(playerPrefab, (Vector2)startingRoom.Positions.RandomElement() * map.TileSize);
            _spawnerManager.SpawnCreature(playerPrefab, (Vector2)startingRoom.Positions.RandomElement() * map.TileSize);
            
            _cameraController.MoveTo(firstUnit.transform.position);
            
            StartCoroutine(WaitToCreateGrid());
        }
        
        private IEnumerator WaitToCreateGrid()
        {
            // TODO: HACK
            yield return new WaitForSeconds(1);
            GridGenerator.FindObjectOfType<GridGenerator>().CreateGrid();
        }
    }
    
    
}