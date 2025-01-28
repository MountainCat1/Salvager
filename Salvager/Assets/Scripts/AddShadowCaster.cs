using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using System.Linq;

public class ShadowGameObjectPool : MonoBehaviour
{
    public Transform poolTransform;
    public GameObject prefab;
    public int poolSize = 0;
    private readonly Dictionary<int, List<GameObject>> objectPool = new();

    private void Update()
    {
        poolSize = 0;
        foreach (var item in objectPool)
        {
            poolSize += item.Value.Count;
        }
    }

    public GameObject GetObjectFromPool(Sprite sprite)
    {
        int hash = CreateHash(sprite);

        if (objectPool.ContainsKey(hash))
        {
            // Retrieve the list of GameObjects associated with the hash.
            List<GameObject> objectsWithSameHash = objectPool[hash];

            // Find the first inactive GameObject in the list, if any.
            GameObject obj = objectsWithSameHash.FirstOrDefault(o => !o.activeInHierarchy);

            if (obj != null)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // Handle the case where the object with the specified hash doesn't exist or is already in use.
        return CreateNewObject(hash, sprite);
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        obj.transform.SetParent(transform, false);
        obj.SetActive(false);
    }

    private int CreateHash(Sprite sprite)
    {
        return sprite.GetHashCode();
    }

    private GameObject CreateNewObject(int hash, Sprite sprite)
    {
        GameObject gameObject = Instantiate(prefab);
        BakeShadow(sprite, gameObject);

        // Add the new GameObject to the list associated with the hash.
        if (objectPool.ContainsKey(hash))
        {
            objectPool[hash].Add(gameObject);
        }
        else
        {
            objectPool.Add(hash, new List<GameObject> { gameObject });
        }

        gameObject.SetActive(true);

        return gameObject;
    }

    private void BakeShadow(Sprite sprite, GameObject obj)
    {
        Vector2[] newShape = GetShape(sprite).ToArray();
        //Vector2[] rescaledShape = RescalePath(newShape);
        Vector3[] newSpritePath = ConvertToVector2Array(newShape);
        obj.TryGetComponent(out ShadowCaster2D shadow);
        shadow.SetShape(newSpritePath);
    }

    private Vector3[] ConvertToVector2Array(Vector2[] vector3Array)
    {
        Vector3[] vector2Array = new Vector3[vector3Array.Length];

        for (int i = 0; i < vector3Array.Length; i++)
        {
            vector2Array[i] = new Vector3(vector3Array[i].x, vector3Array[i].y);
        }

        return vector2Array;
    }

    private List<Vector2> GetShape(Sprite sprite)
    {
        // Get the number of shapes in the Sprite.
        int shapeCount = sprite.GetPhysicsShapeCount();

        if (shapeCount == 0)
        {
            return null;
        }

        List<Vector2> path = new List<Vector2>();

        // only get the first shape. Tiled Sprites are not supported here.
        sprite.GetPhysicsShape(0, path);

        return path;
    }

    /*
    private Vector2[] RescalePath(Vector2[] path)
    {
        if (resizeFactor == initialSizeFactor)
        {
            return path;
        }

        for (int i = 0; i < path.Length; i++)
        {
            var percent = resizeFactor / initialSizeFactor;
            path[i] *= percent;
        }

        return path;
    }
    */
}