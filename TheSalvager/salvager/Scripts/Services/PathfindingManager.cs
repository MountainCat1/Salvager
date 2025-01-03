using Godot;
using System.Collections.Generic;using System.Reflection;

public partial class PathfindingManager : Node2D //, IPathfindingManager
{
    private NavigationRegion2D _navigationRegion = null!;

    public override void _Ready()
    {
        // Assuming a NavigationRegion2D node is a child of this PathfindingManager
        _navigationRegion = GetNode<NavigationRegion2D>("NavigationRegion2D");
        if (_navigationRegion == null)
        {
            GD.PrintErr("NavigationRegion2D node not found. Ensure it is a child of PathfindingManager.");
        }
    }

    public List<Vector2> FindPath(Vector2 start, Vector2 end)
    {
        if (_navigationRegion?.GetNavigationMap() == null)
        {
            GD.PrintErr("NavigationRegion2D or its NavigationMap is not initialized.");
            return new List<Vector2>();
        }

        var path = NavigationServer2D.MapGetPath(
            _navigationRegion.GetNavigationMap(),
            start,
            end,
            optimize: true
        );

        return new List<Vector2>(path);
    }
}