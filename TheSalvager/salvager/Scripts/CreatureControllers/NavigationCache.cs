using System;
using Godot;

namespace CreatureControllers;

public class NavigationCache
{
    public NavigationAgent2D Agent2D { get; }
    private Vector2 _targetPosition;
    private DateTime _lastPathFindTime = DateTime.MinValue;
    private const float PathFindInterval = 0.5f;

    public void SetTargetPosition(Vector2 position)
    {
        if (position == _targetPosition)
            return;

        if ((DateTime.Now - _lastPathFindTime).TotalSeconds < PathFindInterval
            && position.DistanceSquaredTo(_targetPosition) < 1)
            return;

        _targetPosition = position;
        Agent2D.TargetPosition = position;
    }

    public NavigationCache(NavigationAgent2D agent2D)
    {
        Agent2D = agent2D;
    }
}