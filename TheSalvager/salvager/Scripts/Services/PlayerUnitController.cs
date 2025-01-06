using Godot;

namespace Services;

public interface IPlayerUnitController
{
}

public partial class PlayerUnitController : Node2D, IPlayerUnitController
{
    [Inject] private ICreatureManager _creatureManager = null!;
    [Inject] private ISelectionManager _selectionManager = null!;

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("move"))
        {
            var selectedCreatures = _selectionManager.SelectedCreatures;

            foreach (var creature in selectedCreatures)
            {
                if (creature.Controller is not UnitController unitController)
                    continue;

                unitController.SetMoveTarget(GetGlobalMousePosition());
            }
            
            GD.Print($"Moving selected creatures: {string.Join(", ", selectedCreatures)}");
        }
    }
}