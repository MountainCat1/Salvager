using System;
using System.Collections.Generic;
using System.Linq;
using Managers.LevelSelector;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Zenject;

namespace UI
{
    public class LevelSelectorUI : MonoBehaviour
    {
        public event Action LocationSelected;

        [Inject] private IRegionGenerator _regionGenerator;
        [Inject] private IRegionManager _regionManager;

        [SerializeField] private LevelEntryUI levelEntryPrefab;
        [SerializeField] private UILineRenderer linePrefab;
        [SerializeField] private UILineRenderer secondaryLinePrefab;

        [SerializeField] private Transform lineParent;

        [SerializeField] private TextMeshProUGUI selectedLevelNameText;
        [SerializeField] private TextMeshProUGUI selectedLevelDescriptionText;

        [SerializeField] private Transform levelsParent;

        private Vector2 _lastScreenSize;

        public Location SelectedLocation { get; private set; }

        // Unity Methods
        private void Start()
        {
            _regionManager.RegionChanged += OnRegionGenerated;
            if (_regionManager.Region != null)
                OnRegionGenerated();
        }

        private void Update()
        {
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);

            if (this._lastScreenSize != screenSize)
            {
                this._lastScreenSize = screenSize;
                OnRegionGenerated();
            }
        }

        // Callbacks
        private void SelectLevel(Location location)
        {
            //

            SelectedLocation = location;
            LocationSelected?.Invoke();
        }

        private void OnRegionGenerated()
        {
            foreach (Transform child in levelsParent)
            {
                Destroy(child.gameObject);
            }

            foreach (var level in _regionManager.Region.Locations)
            {
                var levelEntry = Instantiate(levelEntryPrefab, levelsParent);
                var distance = _regionManager.GetDistance(_regionManager.CurrentLocationId, level.Id);
                levelEntry.Initialize(level, SelectLevel, distance);

                var rectTransform = levelEntry.GetComponent<RectTransform>();

                // Extract X and Y percentages from the Vector2 key
                Vector2 positionPercent = level.Position;
                float xPercentage = positionPercent.x; // 0 = left, 1 = right
                float yPercentage = positionPercent.y; // 0 = bottom, 1 = top

                // Set anchors based on percentage position
                rectTransform.anchorMin = new Vector2(xPercentage, yPercentage);
                rectTransform.anchorMax = new Vector2(xPercentage, yPercentage);
                rectTransform.pivot = new Vector2(0.5f, 0.5f); // Center pivot

                // Reset position offset
                rectTransform.anchoredPosition = Vector2.zero;
            }


            foreach (Transform child in lineParent)
            {
                Destroy(child.gameObject);
            }

            var levelUIComponents = levelsParent.GetComponentsInChildren<LevelEntryUI>();

            var createdConnections = new List<(Location, Location)>();
            foreach (var level in _regionManager.Region.Locations.OrderBy(x => x.DistanceToCurrent))
            {
                UILineRenderer prefab = null;

                var distanceToCurrent = level.DistanceToCurrent;

                if (distanceToCurrent == 0)
                    prefab = linePrefab;
                if (distanceToCurrent == 1)
                    prefab = secondaryLinePrefab;
                if (distanceToCurrent > 1)
                    continue;

                var uiComponent = Array.Find(levelUIComponents, x => x.Location == level);
                foreach (var connectionLevel in level.Neighbours)
                {
                    if (createdConnections.Contains((connectionLevel, level)))
                        continue;

                    var connectionUIComponent = Array.Find(levelUIComponents, x => x.Location == connectionLevel);

                    var lineRendererInstance = Instantiate(prefab, lineParent);
                    lineRendererInstance.transform.position = Vector3.zero;

                    lineRendererInstance.Points = new[]
                    {
                        (Vector2)lineParent.InverseTransformPoint(uiComponent.transform.position),
                        (Vector2)lineParent.InverseTransformPoint(connectionUIComponent.transform.position)
                    };


                    createdConnections.Add((level, connectionLevel));
                }
            }
        }
    }
}