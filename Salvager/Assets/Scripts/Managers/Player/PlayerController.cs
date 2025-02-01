using System;
using System.Collections.Generic;
using System.Linq;
using CreatureControllers;
using Managers;
using UI;
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
    }

    private void OnDestroy()
    {
        _inputMapper.OnWorldPressed2 -= OnMoveCommand;
        _inputManager.GoBackToMenu -= OnGoBackToMenu;
    }

    private void OnGoBackToMenu()
    {
        SceneManager.LoadScene("Scenes/Menu");
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

        var spreadPositions = IPathfinding.GetSpreadPosition(position, selectedCreatures.Count, LayerMask.GetMask("Walls"), 1f);
        if (!spreadPositions.Any())
        {
            Debug.LogWarning("No valid spread positions found. Using rounded position instead.");
            var roundedPosition = (position - Vector2.one).RoundToNearest(0.5f);
            spreadPositions = IPathfinding.GetSpreadPosition(roundedPosition, selectedCreatures.Count, LayerMask.GetMask("Walls"), 1f);
        }
        if (!spreadPositions.Any())
        {
            Debug.LogWarning("No valid spread positions found. Using smaller radius.");
            spreadPositions = IPathfinding.GetSpreadPosition(position, selectedCreatures.Count, LayerMask.GetMask("Walls"), 0.75f);
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
    /// Assigns target positions to creatures based on the shortest distance.
    /// Ensures each target is assigned only once.
    /// </summary>
    /// <param name="creatures">List of creatures to assign positions to.</param>
    /// <param name="positions">List of target positions.</param>
    /// <returns>A dictionary mapping creatures to their assigned positions.</returns>
    private Dictionary<Creature, Vector2> AssignPositionsToUnits(List<Creature> creatures, List<Vector2> positions)
    {
        var assignments = new Dictionary<Creature, Vector2>();
        var availablePositions = new HashSet<Vector2>(positions);

        foreach (var creature in creatures)
        {
            if (!availablePositions.Any())
                break;

            var currentPos = (Vector2)creature.transform.position;

            // Find the closest available position
            var closestPosition = availablePositions.OrderBy(pos => Vector2.Distance(currentPos, pos)).FirstOrDefault();

            // Assign it to the creature and remove from available positions
            assignments[creature] = closestPosition;
            availablePositions.Remove(closestPosition);
        }

        return assignments;
    }


    private IEnumerable<Creature> GetSelectedCreatures()
    {
        return _selectionManager.SelectedCreatures;
    }
}