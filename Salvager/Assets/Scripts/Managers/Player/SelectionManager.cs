using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using Zenject;

namespace Managers
{
    public interface ISelectionManager
    {
        IEnumerable<Creature> SelectedCreatures { get; }
        event Action OnSelectionChanged;
        void ClearSelection();
        void AddToSelection(Creature creature);
        void RemoveFromSelection(Creature creature);
    }

    public class SelectionManager : MonoBehaviour, ISelectionManager
    {
        public event Action OnSelectionChanged;

        [Inject] ITeamManager _teamManager;
        [Inject] IPoolingManager _poolingManager;

        public IEnumerable<Creature> SelectedCreatures => GetSelectedCreatures();

        [SerializeField] private List<Creature> _selectedCreatures = new();
        [SerializeField] private RectTransform selectionBox;
        [SerializeField] private Canvas canvas;
        [SerializeField] private SelectionMarker selectionMarkerPrefab;
        [SerializeField] private Teams playerTeam;

        private Vector2 _startMousePosition;
        private Camera _camera;
        private IPoolAccess<SelectionMarker> _selectionCirclesPool;

        private const float DragThreshold = 5f; // Threshold in pixels to differentiate between click and drag

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Start()
        {
            _selectionCirclesPool = _poolingManager.GetPoolAccess<SelectionMarker>();
        }

        private void Update()
        {
            HandleSelectionInput();
        }

        private void HandleSelectionInput()
        {
            // Start drag selection
            if (Input.GetMouseButtonDown(0))
            {
                _startMousePosition = Input.mousePosition;
                
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                
                if (selectionBox != null)
                {
                    selectionBox.gameObject.SetActive(true);
                }
            }

            // While dragging
            if (Input.GetMouseButton(0) && selectionBox != null)
            {
                if (Vector2.Distance(_startMousePosition, Input.mousePosition) > DragThreshold)
                {
                    UpdateSelectionBox(Input.mousePosition);
                }
            }

            // Release drag selection or detect click
            if (Input.GetMouseButtonUp(0))
            {
                if (selectionBox != null)
                {
                    selectionBox.gameObject.SetActive(false);
                    selectionBox.sizeDelta = Vector2.zero;
                }

                if (Vector2.Distance(_startMousePosition, Input.mousePosition) <= DragThreshold)
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                        return;

                    HandleSingleClick();
                }
                else
                {
                    if (IsPointerOverInteractiveUI())
                        return;
                    
                    SelectUnitsWithinBox();
                }
            }

            // Shift + Click to add/remove single unit
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Creature creature = hit.collider.GetComponent<Creature>();
                    if (creature != null)
                    {
                        ToggleSelection(creature);
                    }
                }
            }
        }

        private void UpdateSelectionBox(Vector2 currentMousePosition)
        {
            var rect = canvas.GetComponent<RectTransform>().rect;

            Vector2 boxStart = _startMousePosition - rect.size / 2;
            Vector2 boxEnd = currentMousePosition - rect.size / 2;


            Vector2 boxSize = boxEnd - boxStart;
            selectionBox.anchoredPosition = boxStart + boxSize / 2;
            selectionBox.sizeDelta = new Vector2(Mathf.Abs(boxSize.x), Mathf.Abs(boxSize.y));
        }


        private void SelectUnitsWithinBox()
        {
            Vector2 min = _startMousePosition;
            Vector2 max = Input.mousePosition;

            Vector2 bottomLeft = Vector2.Min(min, max);
            Vector2 topRight = Vector2.Max(min, max);

            List<Creature> selectedCreatures = FindObjectsOfType<Creature>()
                .Where(creature => IsWithinSelectionBounds(creature, bottomLeft, topRight))
                .ToList();
            
            if (!Input.GetKey(KeyCode.LeftShift)) // Replace current selection unless Shift is held
            {
                ClearSelection();
            }

            foreach (var creature in selectedCreatures)
            {
                AddToSelection(creature);
            }
            
            
            OnSelectionChanged?.Invoke();
        }

        private void HandleSingleClick()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Creature creature = hit.collider.GetComponent<Creature>();
                if (creature != null)
                {
                    ClearSelection(); // Deselect all and select the clicked creature
                    AddToSelection(creature);
                    
                    OnSelectionChanged?.Invoke();
                }
                else
                {
                    ClearSelection(); // Deselect all if no creature is clicked
                    
                    OnSelectionChanged?.Invoke();
                }
            }
            else
            {
                ClearSelection(); // Deselect all if click hits nothing
                
                OnSelectionChanged?.Invoke();
            }
        }

        private bool IsWithinSelectionBounds(Creature creature, Vector2 bottomLeft, Vector2 topRight)
        {
            Vector3 screenPos = _camera.WorldToScreenPoint(creature.transform.position);

            return screenPos.x >= bottomLeft.x && screenPos.x <= topRight.x &&
                   screenPos.y >= bottomLeft.y && screenPos.y <= topRight.y;
        }

        private void ToggleSelection(Creature creature)
        {
            if (_selectedCreatures.Contains(creature))
            {
                RemoveFromSelection(creature);
            }
            else
            {
                AddToSelection(creature);
            }
        }

        // Public Methods
        public void ClearSelection()
        {
            foreach (var selectionCircle in _selectionCirclesPool.GetInUseObjects().ToList())
            {
                _selectionCirclesPool.DespawnObject(selectionCircle);
            }

            _selectedCreatures.Clear();
        }

        public void AddToSelection(Creature creature)
        {
            if (creature.Team != playerTeam)
                return;

            if (!_selectedCreatures.Contains(creature))
            {
                _selectedCreatures.Add(creature);
                
                var selectionMarker = _selectionCirclesPool.SpawnObject(selectionMarkerPrefab, creature.transform.position);
                selectionMarker.ParentConstraint.AddSource(new ConstraintSource()
                {
                    sourceTransform = creature.transform,
                    weight = 1
                });
                selectionMarker.Creature = creature;
            }
        }

        public void RemoveFromSelection(Creature creature)
        {
            if (_selectedCreatures.Contains(creature))
            {
                _selectedCreatures.Remove(creature);
                
                var selectionMarker = _selectionCirclesPool.GetInUseObjects()
                    .FirstOrDefault(marker => marker.Creature == creature);
                
                _selectionCirclesPool.DespawnObject(selectionMarker);
            }

            OnSelectionChanged?.Invoke();
        }

        private IEnumerable<Creature> GetSelectedCreatures()
        {
            _selectedCreatures.RemoveAll(creature => creature == null);

            return _selectedCreatures;
        }
        
        private bool IsPointerOverInteractiveUI()
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            // Filter out the selection box or other non-interactive elements
            foreach (var result in results)
            {
                if (result.gameObject == selectionBox.gameObject)
                {
                    // Ignore the selection box
                    continue;
                }

                return true;
            }

            return false;
        }

    }
}