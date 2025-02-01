using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Constants;
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
        [Inject] private ICreatureManager _creatureManager;

        [SerializeField] private Creature playerPrefab;
        [SerializeField] private Creature enemyPrefab;

        private MapData _map;
        
        private void Start()
        {
            StartCoroutine(WaitToCreateGrid());
        }
        
        private IEnumerator WaitToCreateGrid()
        {
            // TODO: HACK
            _mapGenerator.GenerateMap();
            _map = _mapGenerator.MapData;
            
            yield return new WaitForSeconds(1);
            GridGenerator.FindObjectOfType<GridGenerator>().CreateGrid();

            yield return new WaitForSeconds(0.5f);

            SpawnUnits();
        }

        private void SpawnUnits()
        {
            var startingRoom = _map.GetAllRooms().First(x => x.IsEntrance);
            
            var playerUnits = new List<Creature>();
            
            playerUnits.Add(_creatureManager.SpawnCreature(playerPrefab, (Vector2)startingRoom.Positions.RandomElement() * _map.TileSize));
            playerUnits.Add(_creatureManager.SpawnCreature(playerPrefab, (Vector2)startingRoom.Positions.RandomElement() * _map.TileSize));
            playerUnits.Add(_creatureManager.SpawnCreature(playerPrefab, (Vector2)startingRoom.Positions.RandomElement() * _map.TileSize));

            foreach (var unit in playerUnits)
            {
                unit.name = $"{Names.Human.RandomElement()} {Surnames.Human.RandomElement()}";
            }
            
            void SpawnEnemyInNonStartingRoom()
            {
                var room = _map.GetAllRooms().Where(x => !x.IsEntrance).RandomElement();
                
                _creatureManager.SpawnCreature(enemyPrefab, (Vector2)room.Positions.RandomElement() * _map.TileSize);
            }

            for (int i = 0; i < 15; i++)
            {
                SpawnEnemyInNonStartingRoom();
            }
            
            _cameraController.MoveTo(playerUnits.First().transform.position);
        }
    }
    
    
}