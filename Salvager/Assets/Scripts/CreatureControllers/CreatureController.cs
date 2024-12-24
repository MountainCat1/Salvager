using UnityEngine;
using Zenject;

[RequireComponent(typeof(Creature))]
public class CreatureController : MonoBehaviour
{
    protected Creature Creature;
    
    [Inject] private IPathfinding _pathfinding;

    protected virtual void Awake()
    {
        Creature = GetComponent<Creature>();
    }

    protected bool CanSee(Creature target)
    {
        var distance = Vector2.Distance(transform.position, target.transform.position); 
        
        // If the target is too far away, we can't see it
        if (distance > Creature.SightRange)
            return false;

        var layerMask = LayerMask.GetMask("Walls");
        var hit = Physics2D.Raycast(
            Creature.transform.position, target.transform.position - Creature.transform.position,
            distance,
            layerMask
        );
        return !hit;
    }
}