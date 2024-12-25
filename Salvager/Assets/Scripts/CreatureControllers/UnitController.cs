using System;
using UnityEngine;

namespace CreatureControllers
{
    public class UnitController : AiController
    {
        private Vector2? _moveCommandTarget;
        private Creature _target;
        private const bool MoveOnAttackCooldown = false; // TODO: This should be a property of the weapon

        public void SetMoveTarget(Vector2 target)
        {
            _moveCommandTarget = target;
            _target = null;
        }

        public void SetAttackTarget(Creature target)
        {
            _target = target;
            _moveCommandTarget = null;
        }

        private void Update()
        {
            if (_moveCommandTarget != null)
            {
                if (Vector2.Distance(Creature.transform.position, _moveCommandTarget.Value) > 0.1f)
                {
                    PerformMovementTowardsPosition(_moveCommandTarget.Value);
                    return;
                }

                _moveCommandTarget = null;
            }

            if (!_target)
                _target = GetNewTarget();

            if (_target)
            {
                var attackContext = new AttackContext()
                {
                    Direction = (_target.transform.position - Creature.transform.position).normalized,
                    Target = _target,
                    Attacker = Creature
                };

                if (Creature.Weapon.GetOnCooldown(attackContext) && !MoveOnAttackCooldown)
                {
                    Creature.SetMovement(Vector2.zero);
                    return;
                }

                if (Vector2.Distance(Creature.transform.position, _target.transform.position) < Creature.Weapon.Range)
                {
                    PerformAttack(attackContext);
                    return;
                }

                PerformMovementTowardsTarget(_target);
                
                return;
            }
            
            // If no target, stop moving
            Creature.SetMovement(Vector2.zero);
        }

        private void PerformAttack(AttackContext ctx)
        {
            Creature.Weapon.ContiniousAttack(ctx);
        }
    }
}