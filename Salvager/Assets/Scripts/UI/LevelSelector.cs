using System;
using Managers;
using Services.MapGenerators;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace UI
{
    [Serializable]
    public class Level
    {
        // Accessors
        public GenerateMapSettings Settings => settings;
        public RoomBlueprint[] RoomBlueprints => roomBlueprints;    
        public string Name => name;
        
        // Serialized fields
        [SerializeField] private GenerateMapSettings settings;
        [SerializeField] private RoomBlueprint[] roomBlueprints;
        [SerializeField] private string name;


        public static Level GenerateRandom(RoomBlueprint[] roomBlueprints)
        {
            var settings = new GenerateMapSettings
            {
                roomCount = UnityEngine.Random.Range(10, 20),
                roomMaxSize = new Vector2Int(UnityEngine.Random.Range(5, 10), UnityEngine.Random.Range(5, 10)),
                roomMinSize = new Vector2Int(UnityEngine.Random.Range(3, 4), UnityEngine.Random.Range(3, 4)),
                gridSize = new Vector2Int(UnityEngine.Random.Range(50, 100), UnityEngine.Random.Range(50, 100))
            };

            var spaceStationNames = new string[]
            {
                "Alpha Station",
                "Ice Breaker",
                "The Ark",
                "The Forge",
                "The Nexus",
                "The Outpost",
                "The Spire",
                "The Vault",
                "The Watchtower",
                "The Workshop",
                "Tranquility",
                "Unity Station",
                "Vanguard",
                "Voyager",
                "Wayfarer",
                "Zenith"
            };
            
            return new Level
            {
                settings = settings,
                roomBlueprints = roomBlueprints,
                name = spaceStationNames[UnityEngine.Random.Range(0, spaceStationNames.Length)]
            };
        }
    }
    
    public class LevelSelector : MonoBehaviour
    {
        [SerializeField] private SceneReference levelScene;
        [SerializeField] private Level[] levels;
        [SerializeField] private RoomBlueprint[] roomBlueprints;
        
        [SerializeField] private LevelEntryUI levelEntryPrefab;
        
        [SerializeField] private TextMeshProUGUI selectedLevelNameText;
        [SerializeField] private TextMeshProUGUI selectedLevelDescriptionText;

        [SerializeField] private Transform levelsParent;
        
        private Level _selectedLevel;
        
        private void Start()
        {
            levels = new[]
            {
                Level.GenerateRandom(roomBlueprints),
                Level.GenerateRandom(roomBlueprints),
                Level.GenerateRandom(roomBlueprints),
                Level.GenerateRandom(roomBlueprints),
                Level.GenerateRandom(roomBlueprints),
            };
                
            
            foreach (var level in levels)
            {
                var levelEntry = Instantiate(levelEntryPrefab, levelsParent);
                levelEntry.Initialize(level, SelectLevel);
            }
        }
        
        public void SelectLevel(Level level)
        {
            Debug.Log($"Selected level: {level.Name}");
            
            // Update UI
            selectedLevelNameText.text = level.Name;
            selectedLevelDescriptionText.text = $"Room count: {level.Settings.roomCount}\n" +
                                                $"Room size: {level.Settings.roomMaxSize}\n" +
                                                $"Room count: {level.Settings.roomCount}";
            //
            _selectedLevel = level;
        }

        public void LoadLevel()
        {
            if (_selectedLevel == null)
            {
                Debug.LogError("No level selected!");
                return;
            }
            
            Debug.Log($"Loading level: {_selectedLevel.Name}");
            
            GameManager.GameSettings = new GameSettings
            {
                Settings = _selectedLevel.Settings,
                RoomBlueprints = _selectedLevel.RoomBlueprints,
                Name = _selectedLevel.Name
            };
            
            // Load level scene
            SceneManager.LoadScene(levelScene);
        }
    }
}