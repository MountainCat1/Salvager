using System.Collections.Generic;
using UnityEngine;

namespace CreatureControllers
{
    public class RangeEnemyController : AiController
    {
        private Creature _target;

        [SerializeField] private bool moveOnAttackCooldown = false;

        private void Update()
        {
            Creature.SetMovement(Vector2.zero);

            if (!_target)
            {
                _target = GetNewTarget();

                if (!_target)
                {
                    return;
                }
            }

            var attackContext = new AttackContext()
            {
                Direction = (_target.transform.position - Creature.Rigidbody2D.transform.position).normalized,
                Target = _target,
                Attacker = Creature
            };


            if (Creature.Weapon.GetOnCooldown(attackContext) && !moveOnAttackCooldown)
                return;


            if (IsInRange(_target, Creature.Weapon.Range) && PathClear(_target, 0.5f)) // TODO: Magic number, its the radius of the creature of a size of a human
            {
                PerformAttack(attackContext);
                return;
            }

            PerformMovementTowardsTarget(_target);
        }

        private void PerformAttack(AttackContext ctx)
        {
            Creature.Weapon.ContiniousAttack(ctx);
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
    }
}