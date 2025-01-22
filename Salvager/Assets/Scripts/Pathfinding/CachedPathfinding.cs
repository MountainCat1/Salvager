using System;
using System.Collections.Generic;
using UnityEngine;

public class CachedPathfinding : IPathfinding
{
    private readonly IPathfinding _innerPathfinding;

    private readonly Dictionary<(Vector3, Vector3), List<Node>> _pathCache = new();
    private readonly Dictionary<(Vector2, Vector2), bool> _clearPathCache = new();
    private readonly Dictionary<Vector2, bool> _walkableCache = new();
    private readonly Dictionary<(Vector2, int), ICollection<Vector2>> _spreadPositionCache = new();

    public CachedPathfinding(IPathfinding innerPathfinding)
    {
        _innerPathfinding = innerPathfinding;
    }

    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        var key = (startPos, targetPos);
        if (_pathCache.TryGetValue(key, out var cachedPath))
        {
            return cachedPath;
        }

        var path = _innerPathfinding.FindPath(startPos, targetPos);
        _pathCache[key] = path;
        return path;
    }

    public bool IsClearPath(Vector2 a, Vector2 b)
    {
        var key = (a, b);
        if (_clearPathCache.TryGetValue(key, out var cachedResult))
        {
            return cachedResult;
        }

        var result = _innerPathfinding.IsClearPath(a, b);
        _clearPathCache[key] = result;
        return result;
    }

    public bool IsWalkable(Vector2 targetPosition)
    {
        if (_walkableCache.TryGetValue(targetPosition, out var cachedResult))
        {
            return cachedResult;
        }

        var result = _innerPathfinding.IsWalkable(targetPosition);
        _walkableCache[targetPosition] = result;
        return result;
    }

    public ICollection<Vector2> GetSpreadPosition(Vector2 position, int amount)
    {
        var key = (position, amount);
        if (_spreadPositionCache.TryGetValue(key, out var cachedResult))
        {
            return cachedResult;
        }

        var result = _innerPathfinding.GetSpreadPosition(position, amount);
        _spreadPositionCache[key] = result;
        return result;
    }

    public void AddObstacle(IObstacle obstacle)
    {
        _innerPathfinding.AddObstacle(obstacle);
        obstacle.Moved += () =>
        {
            Debug.Log("Obstacle moved - invalidating nmav cache");
            InvalidateCache(obstacle.Position, obstacle.Radius);
        };
        ClearCache();
    }

    public void RemoveObstacle(IObstacle obstacle)
    {
        _innerPathfinding.RemoveObstacle(obstacle);
        ClearCache();
    }

    public void InvalidateCache(Vector2 position, float radius)
    {
        // Remove affected paths
        var keysToRemovePath = new List<(Vector3, Vector3)>();
        foreach (var key in _pathCache.Keys)
        {
            if (Vector2.Distance((Vector2)key.Item1, position) <= radius ||
                Vector2.Distance((Vector2)key.Item2, position) <= radius)
            {
                keysToRemovePath.Add(key);
            }
        }
        foreach (var key in keysToRemovePath)
        {
            _pathCache.Remove(key);
        }

        // Remove affected clear path results
        var keysToRemoveClearPath = new List<(Vector2, Vector2)>();
        foreach (var key in _clearPathCache.Keys)
        {
            if (Vector2.Distance(key.Item1, position) <= radius ||
                Vector2.Distance(key.Item2, position) <= radius)
            {
                keysToRemoveClearPath.Add(key);
            }
        }
        foreach (var key in keysToRemoveClearPath)
        {
            _clearPathCache.Remove(key);
        }

        // Remove affected walkable results
        var keysToRemoveWalkable = new List<Vector2>();
        foreach (var key in _walkableCache.Keys)
        {
            if (Vector2.Distance(key, position) <= radius)
            {
                keysToRemoveWalkable.Add(key);
            }
        }
        foreach (var key in keysToRemoveWalkable)
        {
            _walkableCache.Remove(key);
        }

        // Remove affected spread position results
        var keysToRemoveSpreadPosition = new List<(Vector2, int)>();
        foreach (var key in _spreadPositionCache.Keys)
        {
            if (Vector2.Distance(key.Item1, position) <= radius)
            {
                keysToRemoveSpreadPosition.Add(key);
            }
        }
        foreach (var key in keysToRemoveSpreadPosition)
        {
            _spreadPositionCache.Remove(key);
        }
    }

    private void ClearCache()
    {
        _pathCache.Clear();
        _clearPathCache.Clear();
        _walkableCache.Clear();
        _spreadPositionCache.Clear();
    }
}
