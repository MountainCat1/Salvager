using System.Collections.Generic;
using Godot;

namespace Utilities;

public static class NodeExtensions
{
    public static List<Node> GetAllChildren(this Node inNode, List<Node>? array = null)
    {
        if (array == null)
            array = new List<Node>();

        array.Add(inNode);

        foreach (Node child in inNode.GetChildren())
        {
            GetAllChildren(child, array);
        }

        return array;
    }
    
    public static List<Node> GetWithAllChildren(this Node inNode, List<Node>? array = null)
    {
        if (array == null)
            array = new List<Node>();

        array.Add(inNode);

        foreach (Node child in inNode.GetChildren())
        {
            GetAllChildren(child, array);
        }

        array.Add(inNode);
        
        return array;
    }
}