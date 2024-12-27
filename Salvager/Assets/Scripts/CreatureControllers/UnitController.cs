using System;
using UnityEngine;

namespace CreatureControllers
{
    public class UnitController : AiController
    {
        private Vector2? _moveCommandTarget;
        private Creature _target;
        private const bool MoveOnAttackCooldown = false; // TODO: This should be a configurable property of the weapon

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
            HandleMovementOrAttack();
        }

        private void HandleMovementOrAttack()
        {
            if (_moveCommandTarget.HasValue)
            {
                HandleMovementToTarget();
                return;
            }

            if (_target == null)
            {
                _target = GetNewTarget();
            }

            if (_target != null)
            {
                HandleAttackOrMovementToTarget();
                return;
            }

            Creature.SetMovement(Vector2.zero); // Stop moving if no target
        }

        private void HandleMovementToTarget()
        {
            if (Vector2.Distance(Creature.transform.position, _moveCommandTarget.Value) > 0.1f)
            {
                PerformMovementTowardsPosition(_moveCommandTarget.Value);
            }
            else
            {
                _moveCommandTarget = null; // Reached the destination
            }
        }

        private void HandleAttackOrMovementToTarget()
        {
            var attackContext = CreateAttackContext();

            if (Creature.Weapon.GetOnCooldown(attackContext) && !MoveOnAttackCooldown)
            {
                Creature.SetMovement(Vector2.zero); // Stay still during cooldown if configured
                return;
            }

            if (Vector2.Distance(Creature.transform.position, _target.transform.position) < Creature.Weapon.Range)
            {
                PerformAttack(attackContext);
            }
            else
            {
                PerformMovementTowardsTarget(_target);
            }
        }

        private AttackContext CreateAttackContext()
        {
            return new AttackContext
            {
                Direction = (_target.transform.position - Creature.transform.position).normalized,
                Target = _target,
                Attacker = Creature
            };
        }

        private void PerformAttack(AttackContext context)
        {
            Creature.Weapon.ContiniousAttack(context);
        }
    }
}
