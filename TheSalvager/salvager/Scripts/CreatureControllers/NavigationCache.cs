using System;
using Godot;

namespace CreatureControllers;

public class NavigationCache
{
    private Vector2 _targetPosition;
    private DateTime _lastPathFindTime = DateTime.MinValue;
    private const float PathFindInterval = 0.5f;

    public Action<Vector2> SetTargetPositionDelegate;
    public Func<Vector2> GetTargetPositionDelegate;

    public void SetTargetPosition(Vector2 position)
    {
        if (GetTargetPositionDelegate() == position)
            return;
        
        if ((DateTime.Now - _lastPathFindTime).TotalSeconds < PathFindInterval
            && position.DistanceSquaredTo(_targetPosition) < 1)
            return;

        _targetPosition = position;
        SetTargetPositionDelegate(position);
    }

    public NavigationCache(Action<Vector2> setTargetPositionDelegate, Func<Vector2> getTargetPositionDelegate)
    {
        SetTargetPositionDelegate = setTargetPositionDelegate;
        GetTargetPositionDelegate = getTargetPositionDelegate;
    }
}