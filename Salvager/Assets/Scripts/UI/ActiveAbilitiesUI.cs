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
        [Inject] private IInputManager _inputManager;
        [Inject] private IInputMapper _inputMapper;

        [SerializeField]
        private AbilityButtonUI abilityButtonUIPrefab;

        [SerializeField]
        private Transform buttonContainer;

        [SerializeField]
        private LayerMask groundLayer;

        [SerializeField]
        private Texture2D targetingCursor;

        [SerializeField]
        private Texture2D defaultCursor;

        // Removed: private Ability _abilityWaitingForTarget;

        void Start()
        {
            _selectionManager.OnSelectionChanged += UpdateUI;
        }

        private void OnEnable()
        {
            _inputManager.OnCancel += CancelAbilityTargeting;
        }

        private void OnDisable()
        {
            _inputManager.OnCancel -= CancelAbilityTargeting;
            _inputMapper.CancelFollowUpClick(); // Ensure cancellation on disable
        }

        private void OnDestroy()
        {
            _selectionManager.OnSelectionChanged -= UpdateUI;
        }

        private void CancelAbilityTargeting()
        {
            _inputMapper.CancelFollowUpClick();
            Debug.Log("Ability use cancelled.");
            _selectionManager.AllowSelection(this);
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
                button.Initialize(
                    activeItem.GetAbility(),
                    activeItem.Icon,
                    (ability) => StartAbilityTargeting(ability)
                ); // Modified callback
            }
        }

        private void StartAbilityTargeting(Ability ability)
        {
            // Set the ability we're waiting for a target for
            // _abilityWaitingForTarget = ability; // Removed
            _selectionManager.PreventSelection(this);
            Debug.Log($"Waiting for target location for ability: {ability.Identifier}");

            _inputMapper.WaitForFollowUpClick((worldPosition) =>
            {
                UseAbilityAtLocation(ability, worldPosition);
                _selectionManager.AllowSelection(this);
            }, targetingCursor);
        }

        private void UseAbilityAtLocation(Ability ability, Vector3 targetLocation)
        {
            // Find first selected creature with the ability
            var creature = _selectionManager.SelectedCreatures.FirstOrDefault(
                x =>
                    x.Inventory.Items.Any(
                        i =>
                            i is ActiveItemBehaviour activeItem
                            && activeItem.GetAbility().Identifier == ability.Identifier
                    )
            );

            if (creature == null)
            {
                Debug.LogError($"No creature with ability {ability.Identifier} selected.");
                return;
            }

            var item = creature.Inventory.Items.First(
                    i =>
                        i is ActiveItemBehaviour activeItem
                        && activeItem.GetAbility().Identifier == ability.Identifier
                ) as ActiveItemBehaviour;

            if (item == null)
            {
                Debug.LogError(
                    $"No item with ability {ability.Identifier} found in creature's inventory."
                );
                return;
            }

            item.UseActiveAbility(new AbilityUseContext(creature, null, targetLocation));

            UpdateUI();
        }
    }
}
