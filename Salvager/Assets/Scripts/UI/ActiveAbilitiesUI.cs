using System;
using System.Linq;
using Items;
using Managers;
using UnityEngine;
using Zenject;

namespace UI
{
    public class ActiveAbilitiesUI : MonoBehaviour
    {
        [Inject] private ISelectionManager _selectionManager;
        [Inject] private DiContainer _diContainer;

        [SerializeField] private AbilityButtonUI abilityButtonUIPrefab;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private LayerMask groundLayer; // Assignable in inspector
        [SerializeField] private Texture2D targetingCursor; // Assignable in inspector
        [SerializeField] private Texture2D defaultCursor; // Assignable in inspector

        private Ability _abilityWaitingForTarget;

        void Start()
        {
            _selectionManager.OnSelectionChanged += UpdateUI;
        }

        private void OnDestroy()
        {
            _selectionManager.OnSelectionChanged -= UpdateUI;
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        }

        private void Update()
        {
            if (_abilityWaitingForTarget != null && Input.GetMouseButtonDown(0))
            {
                // Handle click to determine world position
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, groundLayer);

                UseAbilityAtLocation(_abilityWaitingForTarget, mousePosition);
                _abilityWaitingForTarget = null; // Clear waiting ability
                _selectionManager.AllowSelection(this);
                SetDefaultCursor();
            }
            else if (_abilityWaitingForTarget != null && Input.GetKeyDown(KeyCode.Escape))
            {
                // Cancel ability use
                _abilityWaitingForTarget = null;
                _selectionManager.AllowSelection(this);
                Debug.Log("Ability use cancelled.");
                SetDefaultCursor();
            }
        }

        private void UpdateUI()
        {
            var activeItems = _selectionManager
                .SelectedCreatures
                .SelectMany(x => x.Inventory.Items)
                .Select(x => x as ActiveItemBehaviour)
                .Where(x => x != null);

            foreach (Transform child in buttonContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (var activeItem in activeItems)
            {
                var button = _diContainer
                    .InstantiatePrefab(abilityButtonUIPrefab, buttonContainer)
                    .GetComponent<AbilityButtonUI>();
                button.Initialize(activeItem.GetAbility(), activeItem.Icon,
                    (ability) => StartAbilityTargeting(ability)); // Modified callback
            }
        }

        private void StartAbilityTargeting(Ability ability)
        {
            // Set the ability we're waiting for a target for
            _abilityWaitingForTarget = ability;
            _selectionManager.PreventSelection(this);
            Debug.Log($"Waiting for target location for ability: {ability.Identifier}");
            SetTargetingCursor();
        }

        private void UseAbilityAtLocation(Ability ability, Vector3 targetLocation)
        {
            // Find first selected creature with the ability
            var creature = _selectionManager.SelectedCreatures.FirstOrDefault(x =>
                x.Inventory.Items.Any(i =>
                    i is ActiveItemBehaviour activeItem &&
                    activeItem.GetAbility().Identifier == ability.Identifier));

            if (creature == null)
            {
                Debug.LogError($"No creature with ability {ability.Identifier} selected.");
                return;
            }

            var item = creature.Inventory.Items.First(i =>
                    i is ActiveItemBehaviour activeItem &&
                    activeItem.GetAbility().Identifier == ability.Identifier) as
                ActiveItemBehaviour;

            if (item == null)
            {
                Debug.LogError(
                    $"No item with ability {ability.Identifier} found in creature's inventory.");
                return;
            }

            item.UseActiveAbility(new AbilityUseContext(creature, null, targetLocation));
            
            UpdateUI();
        }

        private void SetTargetingCursor()
        {
            Cursor.SetCursor(targetingCursor, Vector2.zero, CursorMode.Auto);
        }

        private void SetDefaultCursor()
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        }
    }
}
