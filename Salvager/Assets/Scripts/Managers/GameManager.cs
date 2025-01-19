using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Services.MapGenerators;
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

        private MapData _map;
        
        private void Start()
        {
            _mapGenerator.GenerateMap();

            _map = _mapGenerator.MapData;
            
            StartCoroutine(WaitToCreateGrid());
        }
        
        private IEnumerator WaitToCreateGrid()
        {
            // TODO: HACK
            yield return new WaitForSeconds(1);
            GridGenerator.FindObjectOfType<GridGenerator>().CreateGrid();

            yield return new WaitForSeconds(0.5f);

            SpawnUnits();
        }

        private void SpawnUnits()
        {
            var startingRoom = _map.GetAllRooms().First(x => x.IsEntrance);
            
            var firstUnit = _spawnerManager.SpawnCreature(playerPrefab, (Vector2)startingRoom.Positions.RandomElement() * _map.TileSize);
            _spawnerManager.SpawnCreature(playerPrefab, (Vector2)startingRoom.Positions.RandomElement() * _map.TileSize);
            _spawnerManager.SpawnCreature(playerPrefab, (Vector2)startingRoom.Positions.RandomElement() * _map.TileSize);
            
            void SpawnEnemyInNonStartingRoom()
            {
                var room = _map.GetAllRooms().Where(x => !x.IsEntrance).RandomElement();
                
                _spawnerManager.SpawnCreature(enemyPrefab, (Vector2)room.Positions.RandomElement() * _map.TileSize);
            }

            for (int i = 0; i < 15; i++)
            {
                SpawnEnemyInNonStartingRoom();
            }
            
            _cameraController.MoveTo(firstUnit.transform.position);
        }
    }
    
    
}