using UnityEngine;

namespace Utilities
{
    public static class CollisionUtility
    {
        public static bool IsObstacle(GameObject go)
        {
            return go.layer == LayerMask.NameToLayer("Walls");
        }
    }
}