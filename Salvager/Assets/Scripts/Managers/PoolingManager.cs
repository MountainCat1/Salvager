// Draft created by MountainCat on 2024-01-20


// using System;
// using System.Collections.Generic;
// using System.Linq;
// using JetBrains.Annotations;
// using Managers;
// using UnityEngine;
// using Zenject;
//
//
// public interface IFreeable
// {
//     public void Deinitialize();
// }
//
// public interface IPoolingManager
// {
//     [CanBeNull] public T SpawnObject<T>(
//         T prefabGameObject,
//         Vector2 position,
//         float rotation = 0,
//         [CanBeNull] GameObject parent = null) where T : MonoBehaviour;
//
//     public void DespawnObject<T>(T node) where T : MonoBehaviour;
//
//     public IReadOnlyCollection<T> GetPooledObjects<T>() where T : MonoBehaviour;
// }
//
// public class PoolingManager : MonoBehaviour, IPoolingManager
// {
//     [Inject] private ISpawnerManager _spawnerManager = null!;
//
//     private Dictionary<Type, ObjectPool> _pooledObjects = new();
//
//     public T? SpawnObject<T>(
//         T prefabGameObject,
//         Vector2 position,
//         float rotation = 0,
//         GameObject? parent = null) where T : MonoBehaviour
//     {
//         var pool = GetPool<T>();
//         var node = pool.GetFreeObject();
//         if (node == null)
//         {
//             node = _spawnerManager.Spawn<T>(prefabGameObject, position);
//             if (node == null)
//                 throw new InvalidOperationException($"Failed to spawn object of type {typeof(T)}");
//
//             Debug.Log($"Added {node.name} to pool ({pool.Count})");
//             pool.AddObject(node);
//             pool.UseObject(node);
//         }
//         else
//         {
//             Debug.Log($"Reusing {node.name} from pool ({pool.Count})");
//             pool.UseObject(node);
//
//             // Call free method if object it has it
//             node.transform.position = position;
//             node.transform.rotation = Quaternion.Euler(0, 0, rotation);
//         }
//
//         return node;
//     }
//
//     public void DespawnObject<T>(T node) where T : MonoBehaviour
//     {
//         var pool = GetPool<T>();
//         pool.FreeObject(node);
//     }
//
//     public IReadOnlyCollection<T> GetPooledObjects<T>() where T : MonoBehaviour
//     {
//         var pool = GetPool<T>();
//         return pool.GetFreeObjects();
//     }
//
//
//     private ObjectPool<T> GetPool<T>() where T : MonoBehaviour
//     {
//         var type = typeof(T);
//         var pool = _pooledObjects.GetValueOrDefault(type) as ObjectPool<T>;
//         if (pool == null)
//         {
//             pool = new ObjectPool<T>();
//             _pooledObjects[type] = pool;
//         }
//
//         return pool;
//     }
// }
//
// public class ObjectPool
// {
// }
//
// public class ObjectPool<T> : ObjectPool where T : MonoBehaviour
// {
//     private const int WarningThreshold = 40;
//
//
//     public int Count => _freeObjects.Count + _inUseObjects.Count;
//
//     private List<T> _inUseObjects = new();
//     private List<T> _freeObjects = new();
//
//     public void AddObject(T node)
//     {
//         _freeObjects.Add(node);
//
//         if (Count > WarningThreshold)
//             Debug.LogWarning($"Pool of type {typeof(T)} has {Count} objects");
//
//         node.gameObject.SetActive(false);
//     }
//
//     public void UseObject(T node)
//     {
//         if (!_freeObjects.Remove(node))
//         {
//             Debug.LogWarning($"Using object which was not in the pool {node.name}");
//         }
//
//         _inUseObjects.Add(node);
//
//         node.gameObject.SetActive(true);
//     }
//
//     public void FreeObject(T node)
//     {
//         if (!_inUseObjects.Remove(node))
//         {
//             Debug.LogWarning($"Freeing object which was not ion the pool {node.name}");
//         }
//
//         _freeObjects.Add(node);
//
//         node.gameObject.SetActive(false);
//
//         var freeable = node as IFreeable;
//         freeable?.Deinitialize();
//     }
//
//     [CanBeNull]
//     public T GetFreeObject()
//     {
//         if (_freeObjects.Count == 0)
//         {
//             return null;
//         }
//
//         return _freeObjects.First();
//     }
//
//     public IReadOnlyCollection<T> GetFreeObjects()
//     {
//         return _freeObjects;
//     }
// }