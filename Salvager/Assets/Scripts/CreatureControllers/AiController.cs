using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using Zenject;

namespace CreatureControllers
{
    public class AiController : CreatureController
    {
        // Events
        protected virtual void Start()
        {
            Creature.Hit += OnHit;
        }

        // Injected Dependencies (using Zenject)
        [Inject] protected IPathfinding Pathfinding;
        [Inject] protected ICreatureManager CreatureManager;

        // Public Constants
        private const double MemoryTime = 20;

        // Static Variables and Methods

        // Public Variables

        // Serialized Private Variables

        // Private Variables
        private Dictionary<Creature, DateTime> _memorizedCreatures = new();

        // Properties

        // Unity Callbacks
        private void FixedUpdate()
        {
            UpdateMemory();
        }

        // Public Methods
        public ICollection<Creature> GetMemorizedCreatures()
        {
            return _memorizedCreatures
                .Select(x => x.Key)
                .Where(x => x) // Filter out null creatures, is it even necessary??? idk...
                .ToList();
        }

        // Virtual Methods

        // Abstract Methods

        // Private Methods
        private void MoveViaPathfinding(Vector2 targetPosition)
        {
            var path = Pathfinding.FindPath(Creature.transform.position, targetPosition);
            if (path.Count == 0)
            {
                return;
            }

            var nextNode = path[0];
            Vector2 direction = nextNode.worldPosition - Creature.transform.position;
            Creature.SetMovement(direction);
            Debug.DrawLine(Creature.transform.position, nextNode.worldPosition, Color.red);
         
            // draw path
            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine(path[i].worldPosition, path[i + 1].worldPosition, Color.red);
            }
        }

        private void MoveStraightToTarget(Creature target)
        {
            MoveStraightToTarget(target.transform.position);
        }

        private void MoveStraightToTarget(Vector2 targetPosition)
        {
            var direction = (targetPosition - (Vector2)Creature.transform.position).normalized;
            Creature.SetMovement(direction);
            Debug.DrawLine(Creature.transform.position, targetPosition, Color.green);
        }
        
        private List<Vector3> GetCornerPoints(Vector3 center, float radius)
        {
            List<Vector3> cornerPoints = new List<Vector3>
            {
                center + new Vector3(radius, 0, 0), // Right
                center + new Vector3(0, radius, 0), // Up
                center + new Vector3(-radius, 0, 0), // Left
                center + new Vector3(0, -radius, 0) // Down
            };
            return cornerPoints;
        }

        private void UpdateMemory()
        {
            var keys = _memorizedCreatures.Keys.ToList();
            foreach (var key in keys)
            {
                if ((DateTime.Now - _memorizedCreatures[key]).TotalSeconds > MemoryTime)
                    _memorizedCreatures.Remove(key);
                
                if (!key)
                    _memorizedCreatures.Remove(key);
            }

            CreatureManager.GetCreatures()
                .Where(CanSee)
                .ToList()
                .ForEach(Memorize);
        }

        private void Memorize(Creature creature)
        {
            _memorizedCreatures[creature] = DateTime.Now;
        }

        // Event Handlers
        private void OnHit(HitContext ctx)
        {
            Memorize(ctx.Attacker);
        }

        // Helper Methods
        protected bool PathClear(Creature target, float radius)
        {
            return PathClear(target.transform.position, radius);

        }
        
        protected bool PathClear(Vector2 targetPosition, float radius)
        {
            Vector3 creaturePosition = Creature.transform.position;
            List<Vector3> cornerPoints = GetCornerPoints(creaturePosition, radius);

            bool pathClear = true;
            foreach (Vector3 corner in cornerPoints)
            {
                if (!Pathfinding.IsClearPath(corner, targetPosition))
                    pathClear = false;

                Debug.DrawLine(corner, targetPosition, Color.blue);
            }

            return pathClear;
        }

        protected void PerformMovementTowardsTarget(Creature target)
        {
            float radius = Creature.GetComponent<CircleCollider2D>().radius;

            var pathClear = PathClear(target, radius);
            if (pathClear)
            {
                Debug.DrawLine(Creature.transform.position, target.transform.position, Color.yellow);
                MoveStraightToTarget(target.transform.position);
                return;
            }
            
            MoveViaPathfinding(target.transform.position);
        }
        
        protected void PerformMovementTowardsPosition(Vector2 position)
        {
            float radius = Creature.GetComponent<CircleCollider2D>().radius;

            var pathClear = PathClear(position, radius);
            if (pathClear)
            {
                Debug.DrawLine(Creature.transform.position, position, Color.yellow);
                MoveStraightToTarget(position);
                return;
            }
            
            MoveViaPathfinding(position);
        }

        protected Creature GetNewTarget()
        {
            var targets = GetMemorizedCreatures()
                .Where(x => Creature.GetAttitudeTowards(x) == Attitude.Hostile)
                // .Where(x => CanSee(x))
                .ToList();

            // Get closest target
            var target = targets
                .OrderBy(x => Vector2.Distance(Creature.transform.position, x.transform.position))
                .FirstOrDefault();

            return target;
        }

        protected bool IsInRange(Creature creature, float range)
        {
            return Vector2.Distance(Creature.transform.position, creature.transform.position) < range;
        }
    }
}
