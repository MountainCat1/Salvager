using System;
using System.Collections.Generic;
using System.Linq;
using CreatureControllers;
using DefaultNamespace.Pathfinding;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using Zenject;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [Inject] IInputMapper _inputMapper;
    [Inject] IInputManager _inputManager;
    [Inject] ISelectionManager _selectionManager;
    [Inject] IPathfinding _pathfinding;


    private void Start()
    {
        _inputMapper.OnWorldPressed2 += OnMoveCommand;
        _inputManager.GoBackToMenu += OnGoBackToMenu;
        _inputManager.Halt += OnHalt;
    }

    private void OnHalt()
    {
        foreach (var creature in GetSelectedCreatures())
        {
            var controller = creature.Controller as UnitController;
            if (controller)
            {
                controller.SetMoveTarget(creature.transform.position); // TODO: This is a hack, should be a stop command
            }
        }
    }

    private void OnDestroy()
    {
        _inputMapper.OnWorldPressed2 -= OnMoveCommand;
        _inputManager.GoBackToMenu -= OnGoBackToMenu;
    }

    private void OnGoBackToMenu()
    {
        SceneManager.LoadScene("Scenes/Level Select");
    }

    private void OnMoveCommand(Vector2 position)
    {
        var entityUnderMouse = _inputMapper.GetEntityUnderMouse();
        if (entityUnderMouse == null)
        {
            MoveCommand(position);
            return;
        }

        var creature = GetSelectedCreatures().FirstOrDefault();
        if (creature == null)
            return;
        
        var unitController = creature.Controller as UnitController;
        if (unitController == null)
            return;
        
        unitController.SetTarget(entityUnderMouse);
    }

    private void MoveCommand(Vector2 position)
    {
        var selectedCreatures = GetSelectedCreatures().ToList();
        if (!selectedCreatures.Any())
            return;
        
        if (selectedCreatures.Count == 1)
        {
            var creature = selectedCreatures.First();
            var controller = creature.Controller as UnitController;
            if (controller)
            {
                controller.SetMoveTarget(position);
            }

            return;
        }

        var spreadPositions = PathfindingUtilities.GetSpreadPosition(position, selectedCreatures.Count, LayerMask.GetMask("Walls"), 1f);
        if (!spreadPositions.Any())
        {
            Debug.LogWarning("No valid spread positions found. Using rounded position instead.");
            var roundedPosition = (position - Vector2.one).RoundToNearest(0.5f);
            spreadPositions = PathfindingUtilities.GetSpreadPosition(roundedPosition, selectedCreatures.Count, LayerMask.GetMask("Walls"), 1f);
        }
        if (!spreadPositions.Any())
        {
            Debug.LogWarning("No valid spread positions found. Using smaller radius.");
            spreadPositions = PathfindingUtilities.GetSpreadPosition(position, selectedCreatures.Count, LayerMask.GetMask("Walls"), 0.75f);
        }
        if (!spreadPositions.Any())
        {
            Debug.LogWarning("No valid spread positions found.");
            return;
        }
        

        // Pair each unit with the closest available target position
        var assignments = AssignPositionsToUnits(selectedCreatures, spreadPositions);

        foreach (var assignment in assignments)
        {
            var creature = assignment.Key;
            var targetPosition = assignment.Value;

            var controller = creature.Controller as UnitController;
            if (controller)
            {
                controller.SetMoveTarget(targetPosition + new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)));
            }
        }

        // Disable collision detection for between selected creatures
        foreach (var creature in selectedCreatures)
        {
            foreach (var otherCreature in selectedCreatures)
            {
                Physics2D.IgnoreCollision(creature.Movement.Collider, otherCreature.Movement.Collider, true);
            }
        }
    }


    /// <summary>
    /// Assigns positions to units in a way that minimizes the total distance between units and their assigned positions.
    /// </summary>
    /// <param name="creatures"></param>
    /// <param name="positions"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public Dictionary<Creature, Vector2> AssignPositionsToUnits(List<Creature> creatures, List<Vector2> positions)
    {
        if (creatures.Count != positions.Count)
            throw new ArgumentException("Number of creatures and positions must be equal.");

        var bestAssignment = new Dictionary<Creature, Vector2>();
        float bestLength = float.MaxValue;

        foreach (var permutation in GetPermutations(positions, positions.Count))
        {
            var assignment = new Dictionary<Creature, Vector2>();
            float totalLength = 0;

            for (int i = 0; i < creatures.Count; i++)
            {
                var creature = creatures[i];
                var position = permutation.ElementAt(i);
                assignment[creature] = position;
                var distance = Vector2.Distance(creature.transform.position, position);
                totalLength += distance * distance;
            }

            if (totalLength < bestLength)
            {
                bestLength = totalLength;
                bestAssignment = new Dictionary<Creature, Vector2>(assignment);
            }
        }

        return bestAssignment;
    }

    private IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
    {
        if (length == 1) return list.Select(t => new T[] { t });

        return list.SelectMany((t, i) => 
            GetPermutations(list.Where((_, index) => index != i), length - 1)
                .Select(tail => (new T[] { t }).Concat(tail)));
    }


    private IEnumerable<Creature> GetSelectedCreatures()
    {
        return _selectionManager.SelectedCreatures;
    }
}