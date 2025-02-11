using UnityEngine;

namespace Utilities
{
    public static class CollisionUtility
    {
        public static bool IsObstacle(GameObject go)
        {
            return go.layer == LayerMask.NameToLayer("Obstacles");
        }
        
        public static bool IsWall(GameObject go)
        {
            return go.layer == LayerMask.NameToLayer("Walls");
        }

        public static int BlockingVisionLayerMask => LayerMask.GetMask("Obstacles", "Walls");
        public static int UnwalkableLayerMask => LayerMask.GetMask("Obstacles", "Walls");
    }
}