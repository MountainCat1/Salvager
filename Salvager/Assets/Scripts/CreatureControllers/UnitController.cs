
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
            if(_moveCommandTarget != null)
            {
                PerformMovementTowardsPosition(_moveCommandTarget.Value);
                return;
            }
            
            if(!_target)
                _target = GetNewTarget();

            if (_target)
            {
                var attackContext = new AttackContext()
                {
                    Direction = (_target.transform.position - Creature.transform.position).normalized,
                    Target = _target,
                    Attacker = Creature
                };
                
                if(Creature.Weapon.GetOnCooldown(attackContext) && !MoveOnAttackCooldown)
                    return;

                if (Vector2.Distance(Creature.transform.position, _target.transform.position) < Creature.Weapon.Range)
                {
                    PerformAttack(attackContext);
                    return;
                }

                PerformMovementTowardsTarget(_target);
            }
        }
        
        private void PerformAttack(AttackContext ctx)
        {
            Creature.Weapon.ContiniousAttack(ctx);
        }
    }
}