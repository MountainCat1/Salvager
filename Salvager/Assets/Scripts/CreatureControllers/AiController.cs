using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Pathfinding;
using UnityEngine;
using Zenject;

namespace CreatureControllers
{
    public class AiController : CreatureController
    {
        protected Seeker Seeker { get; private set; }

        // Events
        protected virtual void Start()
        {
            Creature.Health.Hit += OnHit;
        }

        protected override void Awake()
        {
            base.Awake();

            Seeker = GetComponent<Seeker>();
            if (Seeker == null)
            {
                Debug.LogError("Seeker component is missing on the Creature.");
            }
        }

        // Injected Dependencies (using Zenject)
        [Inject] protected IPathfinding Pathfinding;
        [Inject] protected ICreatureManager CreatureManager;

        // Public Constants
        private const double MemoryTime = 20;

        // Private Variables
        private Dictionary<Creature, long> _memorizedCreatures = new();

        // Unity Callbacks
        private void FixedUpdate()
        {
            UpdateMemory();
        }

        // Public Methods
        public IEnumerable<Creature> GetMemorizedCreatures()
        {
            return _memorizedCreatures
                .Select(x => x.Key)
                .Where(x => x);
        }

        // Private Methods
        private void MoveViaPathfinding(Vector2 targetPosition)
        {
            if (Seeker == null)
            {
                Debug.LogError("Seeker is not assigned.");
                return;
            }

            // Request a path from the current position to the target
            Seeker.StartPath(Creature.transform.position, targetPosition, OnPathComplete);
        }

        private void OnPathComplete(Path p)
        {
            if (p.error || p.vectorPath.Count == 0)
            {
                Debug.LogError("Pathfinding failed or returned an empty path.");
                Creature.SetMovement(Vector2.zero);
                return;
            }

            // Get the next waypoint in the path
            var nextNode = p.vectorPath[1]; // [0] is the current position
            Vector2 direction = ((Vector3)nextNode - Creature.transform.position).normalized;

            if (Vector2.Distance(Creature.transform.position, nextNode) < 0.1f)
            {
                Creature.SetMovement(Vector2.zero);
                return;
            }

            // Move the creature in the direction of the next node
            Creature.SetMovement(direction);

            // Draw debug lines for the path
            Debug.DrawLine(Creature.transform.position, nextNode, Color.red);
            for (int i = 0; i < p.vectorPath.Count - 1; i++)
            {
                Debug.DrawLine(p.vectorPath[i], p.vectorPath[i + 1], Color.green);
            }
        }

        private void MoveStraightToTarget(Creature target)
        {
            MoveStraightToTarget(target.transform.position);
        }

        private void MoveStraightToTarget(Vector2 targetPosition)
        {
            var direction = (targetPosition - (Vector2)Creature.transform.position).normalized;

            if (Vector2.Distance(Creature.transform.position, targetPosition) < 0.1f)
            {
                Creature.SetMovement(Vector2.zero);
                return;
            }

            Creature.SetMovement(direction);
            Debug.DrawLine(Creature.transform.position, targetPosition, Color.green);
        }

        protected List<Vector3> GetCornerPoints(Vector3 center, float radius)
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
            long currentTicks = Environment.TickCount;
            var keysToRemove = new List<Creature>();

            foreach (var kvp in _memorizedCreatures)
            {
                if ((currentTicks - kvp.Value) > MemoryTime * 1000 || !kvp.Key)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _memorizedCreatures.Remove(key);
            }

            foreach (var creature in CreatureManager.GetCreatures())
            {
                if (CanSee(creature))
                {
                    Memorize(creature);
                }
            }
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

        private void Memorize(Creature creature)
        {
            _memorizedCreatures[creature] = Environment.TickCount;
        }

        protected void PerformMovementTowardsTarget(Creature target)
        {
            float radius = Creature.MovementCollider.radius;

            if (PathClear(target, radius))
            {
                Debug.DrawLine(Creature.transform.position, target.transform.position, Color.yellow);
                MoveStraightToTarget(target.transform.position);
            }
            else
            {
                MoveViaPathfinding(target.transform.position);
            }
        }

        protected void PerformMovementTowardsPosition(Vector2 position)
        {
            float radius = Creature.MovementCollider.radius;

            if (PathClear(position, radius))
            {
                Debug.DrawLine(Creature.transform.position, position, Color.yellow);
                MoveStraightToTarget(position);
            }
            else
            {
                MoveViaPathfinding(position);
            }
        }

        protected Creature GetNewTarget()
        {
            var targets = GetMemorizedCreatures()
                .Where(x => Creature.GetAttitudeTowards(x) == Attitude.Hostile);

            return targets
                .OrderBy(x => Vector2.Distance(Creature.transform.position, x.transform.position))
                .FirstOrDefault();
        }

        protected bool IsInRange(Creature creature, float range)
        {
            return Vector2.Distance(Creature.transform.position, creature.transform.position) < range;
        }
    }
}
