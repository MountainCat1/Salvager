using Godot;

namespace CreatureControllers;

public class NavigationCache
{
    public NavigationAgent2D Agent2D { get; }
    private Vector2 _targetPosition;

    public void SetTargetPosition(Vector2 position)
    {
        if(position == _targetPosition)
            return;
        
        _targetPosition = position;
        Agent2D.TargetPosition = position;
    }

    public NavigationCache(NavigationAgent2D agent2D)
    {
        Agent2D = agent2D;
    }
}