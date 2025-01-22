using UnityEngine;
using Zenject;

public class Obstacle : MonoBehaviour, IObstacle
{
    [Inject] IPathfinding _pathfinding;


    // Implementing IObstacle
    Vector2 IObstacle.Position => transform.position;
    float IObstacle.Radius => radius;
    
    [SerializeField] private float radius;
    
    private void OnEnable()
    {
        _pathfinding.AddObstacle(this);
    }
    
    private void OnDisable()
    {
        _pathfinding.RemoveObstacle(this);
    }
}
