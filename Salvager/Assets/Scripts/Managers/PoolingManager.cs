using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Managers;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;


public interface IFreeable
{
    public void Deinitialize();
    public void Initialize(Action free);
}

public interface IPoolingManager
{
    public T SpawnObject<T>(
        T prefabGameObject,
        Vector2 position,
        float rotation = 0,
        [CanBeNull] Transform parent = null) where T : Component;
    
    public T SpawnObject<T>(
        T prefabGameObject,
        string poolName,
        Vector2 position,
        float rotation = 0,
        [CanBeNull] Transform parent = null) where T : Component;

    public void DespawnObject<T>(T node) where T : Component;
    public void DespawnObject<T>(T node, string poolName) where T : Component;

    public IReadOnlyCollection<T> GetInUseObjects<T>() where T : Component;
    public IReadOnlyCollection<T> GetInUseObjects<T>(string poolName) where T : Component;
    
    [Pure]
    public IPoolAccess<T> GetPoolAccess<T>() where T : Component;
}

public interface IPoolAccess<T> where T : Component
{
    T SpawnObject(T prefabGameObject, Vector2 position, float rotation = 0, Transform parent = null);
    void DespawnObject(T node);
    IReadOnlyCollection<T> GetInUseObjects();
}

public class PoolAccess<T> : IPoolAccess<T> where T : Component
{
    private IPoolingManager _poolingManager;
    private string _poolName;
    
    public PoolAccess(IPoolingManager poolingManager, string poolName)
    {
        _poolingManager = poolingManager;
        _poolName = poolName;   
    }
    
    public T SpawnObject(T prefabGameObject, Vector2 position, float rotation = 0, Transform parent = null)
    {
        return _poolingManager.SpawnObject(prefabGameObject, _poolName, position, rotation, parent);
    }
    
    public void DespawnObject(T node)
    {
        _poolingManager.DespawnObject(node, _poolName);
    }

    public IReadOnlyCollection<T> GetInUseObjects()
    {
        return _poolingManager.GetInUseObjects<T>(_poolName);
    }
}

public class PoolingManager : MonoBehaviour, IPoolingManager
{
    [Inject] private ISpawnerManager _spawnerManager = null!;

    private Dictionary<string, ObjectPool> _pooledObjects = new();

    public T SpawnObject<T>(
        T prefabGameObject,
        Vector2 position,
        float rotation = 0,
        Transform parent = null) where T : Component
    {
        return SpawnObject(prefabGameObject, prefabGameObject.GetType().FullName!, position, rotation, parent);
    }

    public T SpawnObject<T>(T prefabGameObject, string poolName, Vector2 position, float rotation = 0, Transform parent = null) where T : Component
    {
        var pool = GetPool<T>(poolName);
        var node = pool.GetFreeObject();
        if (node == null)
        {
            node = _spawnerManager.Spawn<T>(prefabGameObject, position, parent);
            if (node == null)
                throw new InvalidOperationException($"Failed to spawn object of type {typeof(T)}");

            Debug.Log($"Added {node.name} to pool ({pool.Count})");
            pool.AddObject(node);
            pool.UseObject(node);
        }
        else
        {
            Debug.Log($"Reusing {node.name} from pool ({pool.Count})");
            pool.UseObject(node);

            // Call free method if object it has it
            node.transform.position = position;
            node.transform.rotation = Quaternion.Euler(0, 0, rotation);
        }

        return node;
    }
    public void DespawnObject<T>(T node, string poolName) where T : Component
    {
        var pool = GetPool<T>(poolName);
        pool.FreeObject(node);
    }

    public void DespawnObject<T>(T node) where T : Component
    {
        var pool = GetPool<T>(typeof(T).FullName);
        pool.FreeObject(node);
    }

    public IReadOnlyCollection<T> GetInUseObjects<T>() where T : Component
    {
        var pool = GetPool<T>(typeof(T).FullName);
        return pool.GetFreeObjects();
    }
    
    public IReadOnlyCollection<T> GetInUseObjects<T>(string poolName) where T : Component
    {
        var pool = GetPool<T>(poolName);
        return pool.GetInUseObjects();
    }

    [Pure]
    public IPoolAccess<T> GetPoolAccess<T>() where T : Component
    {
        return new PoolAccess<T>(this, Guid.NewGuid().ToString());
    }

    private ObjectPool<T> GetPool<T>(string poolName) where T : Component
    {
        var type = typeof(T);
        var pool = _pooledObjects.GetValueOrDefault(poolName) as ObjectPool<T>;
        if (pool == null)
        {
            pool = new ObjectPool<T>();
            _pooledObjects[poolName] = pool;
        }

        return pool;
    }
}

public class ObjectPool
{
}

public class ObjectPool<T> : ObjectPool where T : Component
{
    private const int WarningThreshold = 40;

    public int Count => _freeObjects.Count + _inUseObjects.Count;

    private List<T> _inUseObjects = new();
    private List<T> _freeObjects = new();

    public void AddObject(T node)
    {
        _freeObjects.Add(node);

        if (Count > WarningThreshold)
            Debug.LogWarning($"Pool of type {typeof(T)} has {Count} objects");

        node.gameObject.SetActive(false);
    }

    public void UseObject(T node)
    {
        if (!_freeObjects.Remove(node))
        {
            Debug.LogWarning($"Using object which was not in the pool {node.name}");
        }

        _inUseObjects.Add(node);

        node.gameObject.SetActive(true);
        if(node is IFreeable freeable)
            freeable.Initialize(() => FreeObject(node));
    }

    public void FreeObject(T node)
    {
        if (!_inUseObjects.Remove(node))
        {
            Debug.LogWarning($"Freeing object which was not ion the pool {node.name}");
        }

        _freeObjects.Add(node);

        node.gameObject.SetActive(false);

        var freeable = node as IFreeable;
        freeable?.Deinitialize();
    }

    [CanBeNull]
    public T GetFreeObject()
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

    public IReadOnlyCollection<T> GetInUseObjects()
    {
        return _inUseObjects;
    }
}