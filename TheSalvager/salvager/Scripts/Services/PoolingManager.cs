using System;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Services;

public interface IFreeable
{
    public void Deinitialize();
}

public interface IPoolingManager
{
    public T? SpawnObject<T>(PackedScene prefabPackedScene, Vector2 position, float rotation = 0,
        Node2D? parent = null) where T : Node2D;

    public void DespawnObject<T>(T node) where T : Node2D;

    public IReadOnlyCollection<T> GetPooledObjects<T>() where T : Node2D;
}

public partial class PoolingManager : Node2D, IPoolingManager
{
    [Inject] private ISpawnerManager _spawnerManager = null!;

    private Dictionary<Type, ObjectPool> _pooledObjects = new();

    public T? SpawnObject<T>(
        PackedScene prefabPackedScene,
        Vector2 position,
        float rotation = 0,
        Node2D? parent = null)
        where T : Node2D
    {
        var pool = GetPool<T>();
        var node = pool.GetFreeObject();
        if (node == null)
        {
            node = _spawnerManager.SpawnCreature<T>(prefabPackedScene, position, rotation, parent);
            if(node == null)
                throw new InvalidOperationException($"Failed to spawn object of type {typeof(T)}");
            
            GD.Print($"Added {node.Name} to pool ({pool.Count})");
            pool.AddObject(node);
            pool.UseObject(node);
        }
        else
        {
            GD.Print($"Reusing {node.Name} from pool ({pool.Count})");
            pool.UseObject(node);
            
            // Call free method if object it has it
            node.Position = position;
            node.Rotation = rotation;
        }
        
        return node;
    }

    public void DespawnObject<T>(T node) where T : Node2D
    {
        var pool = GetPool<T>();
        pool.FreeObject(node);
    }

    public IReadOnlyCollection<T> GetPooledObjects<T>() where T : Node2D
    {
        var pool = GetPool<T>();
        return pool.GetFreeObjects();
    }


    private ObjectPool<T> GetPool<T>() where T : Node2D
    {
        var type = typeof(T);
        var pool = _pooledObjects.GetValueOrDefault(type) as ObjectPool<T>;
        if(pool == null)
        {
            pool = new ObjectPool<T>();
            _pooledObjects[type] = pool;
        }
        return pool;
    }
}

public class ObjectPool
{
}

public class ObjectPool<T> : ObjectPool where T : Node2D
{
    private const int WarningThreshold = 40;
    
    
    public int Count => _freeObjects.Count + _inUseObjects.Count;
    
    private List<T> _inUseObjects = new();
    private List<T> _freeObjects = new();

    public void AddObject(T node)
    {
        _freeObjects.Add(node);
        
        if(Count > WarningThreshold)
            GD.PushWarning($"Pool of type {typeof(T)} has {Count} objects");
        
        node.SetProcess(false);
        node.SetPhysicsProcess(false);  
        node.Visible = false;
    }

    public void UseObject(T node)
    {
        if (!_freeObjects.Remove(node))
        {
            GD.PushWarning($"Using object which was not in the pool {node.Name}");
        }
        _inUseObjects.Add(node);
        
        node.SetProcess(true);
        node.SetPhysicsProcess(true);  
        node.Visible = true;
    }

    public void FreeObject(T node)
    {
        if (!_inUseObjects.Remove(node))
        {
            GD.PushWarning($"Freeing object which was not ion the pool {node.Name}");
        }
        
        _freeObjects.Add(node);
        
        node.SetProcess(false);
        node.SetPhysicsProcess(false);  
        node.Visible = false;
        
        var freeable = node as IFreeable;
        freeable?.Deinitialize();

    }

    public T? GetFreeObject()
    {
        if (_freeObjects.Count == 0)
        {
            return null;
        }

        return _freeObjects.First();
    }

    public IReadOnlyCollection<T> GetFreeObjects()
    {
        return _freeObjects;
    }
}