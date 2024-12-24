using UnityEngine;

namespace Utilities
{
    public static class Vector3Utilities
    {
        public static Vector3 GetRandomPositionOnCircleEdge(Vector3 center, float radius)
        {
            float randomAngle = Random.Range(0f, 360f);
            float spawnX = center.x + radius * Mathf.Cos(Mathf.Deg2Rad * randomAngle);
            float spawnY = center.y + radius * Mathf.Sin(Mathf.Deg2Rad * randomAngle);

            return new Vector3(spawnX, spawnY, center.z);
        }
    }
}