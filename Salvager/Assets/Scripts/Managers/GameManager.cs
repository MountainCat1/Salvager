using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Constants;
using Data;
using Items;
using Services.MapGenerators;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using Zenject;

namespace Managers
{
    public class GameSettings
    {
        public LocationData Location { get; set; }
        public GenerateMapSettings Settings { get; set; }
        public string Name { get; set; }

    }
    
    public class GameManager : MonoBehaviour
    {
        public static GameSettings GameSettings { get; set; }
        
        [Inject] private IMapGenerator _mapGenerator;
        [Inject] private DiContainer _container;
        [Inject] private ISpawnerManager _spawnerManager;
        [Inject] private ICameraController _cameraController;
        [Inject] private ICreatureManager _creatureManager;
        [Inject] private IDataManager _dataManager;
        [Inject] private IVictoryConditionManager _victoryConditionManager;
        [Inject] private IInputManager _inputManager;
        [Inject] private IEnemySpawner _enemySpawner;
        [Inject] private IJuiceManager _juiceManager;

        [SerializeField] private Creature playerPrefab;
        [SerializeField] private Creature enemyPrefab;

        [SerializeField] private List<ItemBehaviour> startingItems;
        [SerializeField] private bool loadDataFromSave = true;

        [SerializeField] private SceneReference levelSelectorScene;
            
        [SerializeField] private float delayToGoBackToLevelSelector = 4f;
        
        private MapData _map;
        private GameData _data;

        private void Start()
        {
            _data = loadDataFromSave ? _dataManager.LoadData() : null;
            
            StartCoroutine(WaitToCreateGrid());
            
            _inputManager.GoBackToMenu += GoBackToLevelSelector;
        }

        private void OnDestroy()
        {
            _inputManager.GoBackToMenu -= GoBackToLevelSelector;
        }

        private void GoBackToLevelSelector()
        {
            SaveCurrentData();

            SceneManager.LoadScene("Scenes/Level Select");
        }

        private IEnumerator WaitToCreateGrid()
        {
            // TODO: HACK, i mean the loading after 1 second, should be done in a better way

            if (GameSettings?.Settings is not null)
            {
                Debug.Log($"Using settings from level selector for map {GameSettings.Name}...");
                _mapGenerator.Settings = GameSettings.Settings;
            }
            else
            {
                Debug.Log("Using settings set in the inspector...");
            }
            
            _mapGenerator.GenerateMap();
            _map = _mapGenerator.MapData;
            
            yield return new WaitForSeconds(1);
            GridGenerator.FindObjectOfType<GridGenerator>().CreateGrid();

            yield return new WaitForSeconds(0.5f);

            SpawnUnits(mapData: _map);
            
            _juiceManager.Initialize(FindObjectsOfType<Creature>().Where(x => x.Team == Teams.Player).ToArray(), _data.Resources.Juice);
            
            yield return new WaitForSeconds(0.5f);
        }

        private void SpawnUnits(MapData mapData)
        {
            var startingRoom = _map.GetAllRooms().First(x => x.IsEntrance);

            var data = loadDataFromSave ? _dataManager.LoadData() : null;
            var playerUnits = new List<Creature>();
            
            if (data is not null)
            {
                foreach (var creatureData in data.Creatures)
                {
                    var creature = _creatureManager.SpawnCreature(playerPrefab, (Vector2)startingRoom.Positions.RandomElement() * _map.TileSize);
                
                    creature.Initialize(creatureData);
                    
                    if(!creatureData.Selected)
                        creature.gameObject.SetActive(false);

                    foreach (var item in creature.Inventory.Items)
                    {
                        item.Use(new ItemUseContext()
                        {
                            Creature = creature
                        });
                    }
                
                    playerUnits.Add(creature);
                }
            }
            else
            {
                playerUnits.Add(_creatureManager.SpawnCreature(playerPrefab, (Vector2)startingRoom.Positions.RandomElement() * _map.TileSize));
                playerUnits.Add(_creatureManager.SpawnCreature(playerPrefab, (Vector2)startingRoom.Positions.RandomElement() * _map.TileSize));
                playerUnits.Add(_creatureManager.SpawnCreature(playerPrefab, (Vector2)startingRoom.Positions.RandomElement() * _map.TileSize));
                
                foreach (var unit in playerUnits)
                {
                    unit.name = $"{Names.Human.RandomElement()} {Surnames.Human.RandomElement()}";
                    
                    foreach (var item in startingItems)
                    {
                        var itemInInventory = unit.Inventory.AddItemFromPrefab(item);
                        itemInInventory.Use(new ItemUseContext()
                        {
                            Creature = unit
                        });
                    }
                }
            }
            
            // void SpawnEnemyInNonStartingRoom()
            // {
            //     var room = _map.GetAllRooms().Where(x => !x.IsEntrance).RandomElement();
            //     
            //     _creatureManager.SpawnCreature(enemyPrefab, (Vector2)room.Positions.RandomElement() * _map.TileSize);
            // }
            
            _enemySpawner.Initialize(mapData: mapData, location: GameSettings.Location);

            // for (int i = 0; i < 15; i++)
            // {
            //     SpawnEnemyInNonStartingRoom();
            // }

            _victoryConditionManager.Check();
            
            _cameraController.MoveTo(playerUnits.First().transform.position);
            
            _victoryConditionManager.VictoryAchieved += () =>
            {
                Debug.Log("Victory Achieved!");
                StartCoroutine(WaitToGoBackToLevelSelector());
            };
        }
        
        IEnumerator WaitToGoBackToLevelSelector()
        {
            yield return new WaitForSeconds(delayToGoBackToLevelSelector);
            SaveCurrentData();
            SceneManager.LoadScene(levelSelectorScene);
        }
        
        private void SaveCurrentData()
        {
            var currentGameData = _dataManager.GetData();
        
            // Save the current level progress
            currentGameData.Creatures = FindObjectsOfType<Creature>(true)
                .Where(x => x.Team == Teams.Player)
                .Select(CreatureData.FromCreature)
                .ToList();
            
            currentGameData.Resources.Juice = _juiceManager.Juice;
        
            _dataManager.SaveData();
        }
    }
    
    
}