using Godot;

namespace Utilities;

public class NodeUtilities
{
    public static T? FindNodeOfType<T>(Node tree) where T : Node
    {
        foreach (Node node in tree.GetTree().Root.GetAllChildren())
        {
            if (node is T typedNode)
            {
                return typedNode;
            }
        }
        return null;
    }
    
    public static T FindRequiredNodeOfType<T>(Node tree) where T : Node
    {
        foreach (Node node in tree.GetTree().Root.GetAllChildren())
        {
            if (node is T typedNode)
            {
                return typedNode;
            }
        }
        throw new System.NullReferenceException($"{typeof(T).Name} not found");
    }

}