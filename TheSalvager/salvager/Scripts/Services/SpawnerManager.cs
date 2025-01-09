using Godot;

namespace Services;

public interface ISpawnerManager
{
    Node2D SpawnCreature(PackedScene prefabPackedScene, Vector2 position, float rotation = 0, Node2D? parent = null);
    public T? SpawnCreature<T>(PackedScene prefabPackedScene, Vector2 position, float rotation = 0,
        Node2D? parent = null) where T : class;
}

public partial class SpawnerManager : Node2D, ISpawnerManager
{
    [Inject] private IDIContext _context = null!;

    public Node2D SpawnCreature(PackedScene prefabPackedScene, Vector2 position, float rotation = 0,
        Node2D? parent = null)
    {
        var createdNode = prefabPackedScene.Instantiate<Node2D>();

        if (createdNode is null)
        {
            throw new System.Exception("Failed to instantiate prefab");
        }

        createdNode.Position = position;
        createdNode.Rotation = rotation;

        if (parent != null)
        {
            parent.AddChild(createdNode);
        }
        else
        {
            AddChild(createdNode);
        }

        _context.FullInject(createdNode);

        GD.Print($"Spawned {createdNode.Name} creature at {position}");

        return createdNode;
    }

    public T? SpawnCreature<T>(PackedScene prefabPackedScene, Vector2 position, float rotation = 0,
        Node2D? parent = null) where T : class
    {
        return SpawnCreature(prefabPackedScene, position, rotation, parent) as T;
    }
}