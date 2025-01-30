using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using VictoryConditions;
using Zenject;

namespace Managers
{
    public interface IVictoryConditionManager
    {
        event Action VictoryConditionsChanged;
        event Action VictoryAchieved;
        public IEnumerable<KeyValuePair<VictoryCondition, bool>>  VictoryConditions { get; }
    }

    public class VictoryConditionManager : MonoBehaviour, IVictoryConditionManager
    {
        // Events
        public event Action VictoryConditionsChanged;
        public event Action VictoryAchieved;

        // Dependencies
        [Inject] private ISpawnerManager _spawnerManager;
        [Inject] private ISignalManager _signalManager;
        [Inject] private DiContainer _diContainer;
        

        // Serialized Variables
        [SerializeField] private List<VictoryCondition> victoryConditions;
        
        // Accessors
        public IEnumerable<KeyValuePair<VictoryCondition, bool>> VictoryConditions => _conditionStates;
        
        // Private Variables
        private readonly Dictionary<VictoryCondition, bool> _conditionStates = new();

        private void Start()
        {
            foreach (var condition in victoryConditions)
            {
                _conditionStates.Add(condition, false);
                _diContainer.Inject(condition);
            }

            _spawnerManager.Spawned += OnSpawned;
            _signalManager.Signaled += (_) => UpdateWinConditions();

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

        private void UpdateWinConditions(bool force = false)
        {
            foreach (var condition in victoryConditions)
            {
                var conditionState = condition.Check();
                _conditionStates[condition] = conditionState;
            }

            VictoryConditionsChanged?.Invoke();
            
            if (_conditionStates.Values.All(x => x))
            {
                VictoryAchieved?.Invoke();
            }
        }
    }
}