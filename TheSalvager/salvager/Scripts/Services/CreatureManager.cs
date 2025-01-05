using System.Collections.Generic;
using Godot;

namespace Services;

public interface ICreatureManager
{
    public IList<Creature> AllCreatures { get; }
}

public partial class CreatureManager : Node2D, ICreatureManager
{
    
    public IList<Creature> AllCreatures => GetAllCreatures();

    private IList<Creature> GetAllCreatures()
    {
        var creatures = new List<Creature>();
        var nodes = GetAllChildren(GetTree().Root);

        foreach (Node node in nodes)
        {
            if (node is Creature creature)
            {
                creatures.Add(creature);
            }
        }

        return creatures;
    }

    private IEnumerable<Node> GetAllChildren(Window root)
    {
        var nodes = new List<Node>();
        nodes.Add(root);

        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            nodes.AddRange(node.GetChildren());
        }

        return nodes;
    }
}