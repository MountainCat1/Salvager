using Godot;

namespace Utilities;

public static class CollisionUtility
{
    public static bool IsObstacle(Node body)
    {
        return body is TileMapLayer;
    }
}