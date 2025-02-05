using System;
using System.Collections.Generic;
using Managers;
using Managers.LevelSelector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI.Extensions;
using Utilities;
using Zenject;

namespace UI
{
    public class LevelSelectorUI : MonoBehaviour
    {
        [Inject] private IRegionGenerator _regionGenerator;
        [Inject] private IRegionManager _regionManager;
        
        [SerializeField] private SceneReference levelScene;
        [SerializeField] private LevelEntryUI levelEntryPrefab;
        [SerializeField] private UILineRenderer lineRenderer;
        [SerializeField] private Transform lineParent;
        
        [SerializeField] private TextMeshProUGUI selectedLevelNameText;
        [SerializeField] private TextMeshProUGUI selectedLevelDescriptionText;

        [SerializeField] private Transform levelsParent;
        
        private Level _selectedLevel;
        
        private void Start()
        {
            _regionManager.RegionChanged += OnRegionGenerated;
            if(_regionManager.Region != null)
                OnRegionGenerated();
        }

        private void Update()
        {
            var levels = _regionManager.Region.Levels;
            
            var levelUIComponents = levelsParent.GetComponentsInChildren<LevelEntryUI>();

            foreach (var level in levels)
            {
                var uiComponent = Array.Find(levelUIComponents, x => x.Level == level.Value);
                foreach (var connection in _regionManager.Region.Connections[level.Key])
                {
                    var connectionLevel = levels[connection];
                    var connectionUIComponent = Array.Find(levelUIComponents, x => x.Level == connectionLevel);
                    
                    Debug.DrawLine(uiComponent.transform.position, connectionUIComponent.transform.position, Color.green);
                }
            }
        }

        private void OnRegionGenerated()
        {
            foreach (Transform child in levelsParent)
            {
                Destroy(child.gameObject);
            }

            foreach (var level in _regionManager.Region.Levels)
            {
                var levelEntry = Instantiate(levelEntryPrefab, levelsParent);
                levelEntry.Initialize(level.Value, SelectLevel);

                var rectTransform = levelEntry.GetComponent<RectTransform>();

                // Extract X and Y percentages from the Vector2 key
                Vector2 positionPercent = level.Key;
                float xPercentage = positionPercent.x; // 0 = left, 1 = right
                float yPercentage = positionPercent.y; // 0 = bottom, 1 = top

                // Set anchors based on percentage position
                rectTransform.anchorMin = new Vector2(xPercentage, yPercentage);
                rectTransform.anchorMax = new Vector2(xPercentage, yPercentage);
                rectTransform.pivot = new Vector2(0.5f, 0.5f); // Center pivot

                // Reset position offset
                rectTransform.anchoredPosition = Vector2.zero;
            }
            
            var levelUIComponents = levelsParent.GetComponentsInChildren<LevelEntryUI>();

            var createdConnections = new List<(Vector2, Vector2)>();
            foreach (var level in _regionManager.Region.Levels)
            {
                var uiComponent = Array.Find(levelUIComponents, x => x.Level == level.Value);
                foreach (var connection in _regionManager.Region.Connections[level.Key])
                {
                    if (createdConnections.Contains((connection, level.Key)))
                        continue;
                    
                    var connectionLevel = _regionManager.Region.Levels[connection];
                    var connectionUIComponent = Array.Find(levelUIComponents, x => x.Level == connectionLevel);
                    
                    var lineRendererInstance = Instantiate(lineRenderer, lineParent);
                    lineRenderer.transform.position = Vector3.zero;
                    
                    lineRendererInstance.Points = new []
                    {
                        (Vector2)uiComponent.transform.position,
                        (Vector2)connectionUIComponent.transform.position
                    };
                    
                    createdConnections.Add((level.Key, connection));
                }
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

        public void Embark()
        {
            
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