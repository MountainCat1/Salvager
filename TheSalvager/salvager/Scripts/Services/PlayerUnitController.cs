using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CreatureControllers;
using Godot;
using Services.Abstractions;
using Utilities;

namespace Services;

public interface IPlayerUnitController
{
}

public partial class PlayerUnitController : Node2D, IPlayerUnitController
{
    [Inject] private ISelectionManager _selectionManager = null!;
    [Inject] private IMapGenerator _mapGenerator = null!;
    [Inject] private IInputManager _inputManager = null!;

    public void Start()
    {
        _inputManager.ClickedEntity += OnClickedEntity;
    }

    private void OnClickedEntity(Entity obj)
    {
        if (obj is not { } entity)
            return;

        if (Input.IsActionPressed("move"))
        {
            var controllers = _selectionManager.SelectedCreaturesControllers;

            if (!controllers.Any())
                return;

            foreach (var controller in controllers)
            {
                controller.SetTarget(entity);
            }
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("move"))
        {
            var selectedCreatures = _selectionManager.SelectedCreatures;
            var spreadPositions = GetSpreadPositions(GetGlobalMousePosition(), selectedCreatures.Count);

            foreach (var creature in selectedCreatures)
            {
                if (creature.Controller is not CreatureControllers.UnitController unitController)
                    continue;

                unitController.SetMoveTarget(spreadPositions[selectedCreatures.IndexOf(creature)]);
            }

            GD.Print($"Moving selected creatures: {string.Join(", ", selectedCreatures)}");
        }
    }


    private List<Vector2> GetSpreadPositions(Vector2 startPosition, int selectedCreaturesCount)
    {
        Debug.Assert(_mapGenerator.MapData != null, "Map data is null!");

        return _mapGenerator.MapData.GetSpreadPositions(startPosition, selectedCreaturesCount, TileType.Floor);
    }
}