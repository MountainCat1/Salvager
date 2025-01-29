using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Managers
{
    public interface IVictoryConditionManager
    {
        event Action VictoryConditionsChanged;
        public IEnumerable<KeyValuePair<VictoryCondition, bool>>  VictoryConditions { get; }
    }

    public class VictoryConditionManager : MonoBehaviour, IVictoryConditionManager
    {
        // Events
        public event Action VictoryConditionsChanged;

        // Dependencies
        [Inject] private ISpawnerManager _spawnerManager;
        [Inject] private DiContainer _diContainer;

        // Serialized Variables
        [SerializeField] private List<VictoryCondition> victoryConditions;
        
        // Accessors
        public IEnumerable<KeyValuePair<VictoryCondition, bool>> VictoryConditions => _conditionStates;
        
        // Private Variables
        private readonly Dictionary<VictoryCondition, bool> _conditionStates = new();

        private void Start()
        {
            Debug.Log("XD"); 
            
            foreach (var condition in victoryConditions)
            {
                _conditionStates.Add(condition, false);
                _diContainer.Inject(condition);
            }

            _spawnerManager.Spawned += OnSpawned;

            VictoryConditionsChanged?.Invoke();
        }

        private void OnSpawned(Component spawnedComponent)
        {
            if (spawnedComponent is Creature creature)
            {
                creature.Health.Death += (_) => UpdateWinConditions();
                creature.Inventory.Changed += () => UpdateWinConditions();
            }
        }

        private void UpdateWinConditions()
        {
            var changed = false;
            foreach (var condition in victoryConditions)
            {
                var conditionState = condition.Check();
                if (_conditionStates[condition] != conditionState)
                {
                    _conditionStates[condition] = conditionState;
                    changed = true;
                }
            }

            if (changed)
                VictoryConditionsChanged?.Invoke();
        }
    }
}