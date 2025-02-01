using System.Collections.Generic;
using Markers;
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

    // TODO: this seems to be performance heavy, consider some optimizations
    public bool CanSee(Creature target)
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


    public ICollection<Creature> GetCreatureInLine(Vector2 towards, float range)
    {
        var layerMask = LayerMask.GetMask("CreatureHit");
        
        var hits = Physics2D.RaycastAll(
            Creature.transform.position, towards - (Vector2)Creature.transform.position,
            range,
            layerMask
        );

        var creatures = new List<Creature>();
        
        foreach (var hit in hits)
        {
            var creature = hit.collider.GetComponent<CreatureCollider>()?.Creature;
            
            if (creature == null)
                continue;
            
            if (creature == Creature)
                continue;
            
            creatures.Add(creature);
        }
        
        return creatures;
    }
    
}