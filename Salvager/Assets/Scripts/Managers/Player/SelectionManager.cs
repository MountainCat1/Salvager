using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using Utilities;
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

        void PreventSelection(object source);
        void AllowSelection(object source);
    }

    public class SelectionManager : MonoBehaviour, ISelectionManager
    {
        public event Action OnSelectionChanged;

        [Inject] private ITeamManager _teamManager;
        [Inject] private IPoolingManager _poolingManager;
        [Inject] private IInputMapper _inputMapper;
        [Inject] private IInputManager _inputManager;

        public IEnumerable<Creature> SelectedCreatures => GetSelectedCreatures();

        [SerializeField] private List<Creature> _selectedCreatures = new();
        [SerializeField] private RectTransform selectionBox;
        [SerializeField] private Canvas canvas;
        [SerializeField] private SelectionMarker selectionMarkerPrefab;
        [SerializeField] private Teams playerTeam;

        private Vector2? _startMousePosition;
        private Camera _camera;
        private IPoolAccess<SelectionMarker> _selectionCirclesPool;
        private List<object> _selectionPreventers = new();

        private bool _isDragging = false;
        private const float DragThreshold = 5f; // Threshold in pixels

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Start()
        {
            _selectionCirclesPool = _poolingManager.GetPoolAccess<SelectionMarker>();
        }

        private void OnEnable()
        {
            _inputManager.Pointer1Pressed += OnPointer1Pressed;
            _inputManager.Pointer1Hold += OnPointer1Hold;
            _inputManager.Pointer1Released += OnPointer1Released; // Use pressed event for release
        }

        private void OnDisable()
        {
            _inputManager.Pointer1Pressed -= OnPointer1Pressed;
            _inputManager.Pointer1Hold -= OnPointer1Hold;
            _inputManager.Pointer1Released -= OnPointer1Released;
        }

        private void OnPointer1Pressed(Vector2 screenPosition)
        {
            if (_selectionPreventers.Count > 0)
            {
                _startMousePosition = null;
                return;
            }

            if (PointerUtilities.IsPointerOverInteractiveUI(selectionBox.gameObject))
                return;

            _startMousePosition = screenPosition;
            _isDragging = false;

            if (selectionBox != null)
            {
                selectionBox.gameObject.SetActive(true);
                selectionBox.sizeDelta = Vector2.zero;
            }
        }

        private void OnPointer1Hold(Vector2 currentMousePosition)
        {
            if (_startMousePosition == null || selectionBox == null)
                return;

            if (Vector2.Distance(_startMousePosition.Value, currentMousePosition) >
                DragThreshold)
            {
                _isDragging = true;
                UpdateSelectionBox(currentMousePosition);
            }
        }

        private void OnPointer1Released(Vector2 screenPosition)
        {
            if (_startMousePosition == null)
                return;

            if (selectionBox != null)
            {
                selectionBox.gameObject.SetActive(false);
                selectionBox.sizeDelta = Vector2.zero;
            }

            if (PointerUtilities.IsPointerOverInteractiveUI(selectionBox.gameObject))
            {
                _startMousePosition = null;
                return;
            }

            if (!_isDragging)
            {
                HandleSingleClick(screenPosition);
            }
            else
            {
                SelectUnitsWithinBox(_startMousePosition.Value, screenPosition);
            }

            _startMousePosition = null;
            _isDragging = false;
        }

        private void UpdateSelectionBox(Vector2 currentMousePosition)
        {
            var rect = canvas.GetComponent<RectTransform>().rect;

            Vector2 boxStart = _startMousePosition!.Value - rect.size / 2;
            Vector2 boxEnd = currentMousePosition - rect.size / 2;

            Vector2 boxSize = boxEnd - boxStart;
            selectionBox.anchoredPosition = boxStart + boxSize / 2;
            selectionBox.sizeDelta = new Vector2(Mathf.Abs(boxSize.x), Mathf.Abs(boxSize.y));
        }

        private void SelectUnitsWithinBox(Vector2 startPosition, Vector2 endPosition)
        {
            Vector2 min = Vector2.Min(startPosition, endPosition);
            Vector2 max = Vector2.Max(startPosition, endPosition);

            List<Creature> selectedCreatures = FindObjectsOfType<Creature>()
                .Where(creature => IsWithinSelectionBounds(creature, min, max))
                .ToList();

            // Shift + Click to add/remove single unit
            if (!_inputManager.IsShiftPressed)
            {
                ClearSelection();
            }

            foreach (var creature in selectedCreatures)
            {
                AddToSelection(creature);
            }

            OnSelectionChanged?.Invoke();
        }

        private void HandleSingleClick(Vector2 screenPosition)
        {
            var creature = _inputMapper.GetCreatureUnderMouse();
            if (creature != null && creature.Team == playerTeam)
            {
                // Shift + Click to add/remove single unit
                if (_inputManager.IsShiftPressed)
                {
                    ToggleSelection(creature);
                }
                else
                {
                    ClearSelection(); // Deselect all and select the clicked creature
                    AddToSelection(creature);
                }

                OnSelectionChanged?.Invoke();
            }
            else
            {
                ClearSelection(); // Deselect all if no creature is clicked
                OnSelectionChanged?.Invoke();
            }
        }

        private bool IsWithinSelectionBounds(Creature creature, Vector2 bottomLeft,
            Vector2 topRight)
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
            foreach (var selectionCircle in _selectionCirclesPool.GetInUseObjects()
                         .ToList())
            {
                _selectionCirclesPool.DespawnObject(selectionCircle);
            }

            _selectedCreatures.Clear();
        }

        public void AddToSelection(Creature creature)
        {
            if (creature.Team != playerTeam)
                return;
            if (_selectedCreatures.Contains(creature))
                return;

            _selectedCreatures.Add(creature);

            PlaceSelectedMarker(creature);

            creature.Health.Death += OnSelectedCreatureDeath;
            creature.Inventory.Changed += OnSelectedCreatureInventoryChanged;
        }

        private void OnSelectedCreatureInventoryChanged()
        {
            OnSelectionChanged?.Invoke();
        }

        private void OnSelectedCreatureDeath(DeathContext ctx)
        {
            RemoveFromSelection(ctx.KilledEntity as Creature);
        }


        public void RemoveFromSelection(Creature creature)
        {
            if (_selectedCreatures.Contains(creature))
            {
                _selectedCreatures.Remove(creature);

                var selectionMarker = _selectionCirclesPool.GetInUseObjects()
                    .FirstOrDefault(marker => marker.Creature == creature);

                _selectionCirclesPool.DespawnObject(selectionMarker);

                creature.Health.Death -= OnSelectedCreatureDeath;
                creature.Inventory.Changed -= OnSelectedCreatureInventoryChanged;
            }

            OnSelectionChanged?.Invoke();
        }

        public void PreventSelection(object source)
        {
            _selectionPreventers.Add(source);
        }

        public void AllowSelection(object source)
        {
            _selectionPreventers.Remove(source);
        }

        private IEnumerable<Creature> GetSelectedCreatures()
        {
            _selectedCreatures.RemoveAll(creature => creature == null);

            return _selectedCreatures;
        }

        private void PlaceSelectedMarker(Creature creature)
        {
            var selectionMarker =
                _selectionCirclesPool.SpawnObject(selectionMarkerPrefab,
                    creature.transform.position);
            selectionMarker.ParentConstraint.AddSource(new ConstraintSource()
            {
                sourceTransform = creature.transform,
                weight = 1
            });
            selectionMarker.Creature = creature;
        }
    }
}