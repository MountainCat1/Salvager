using System;
using UnityEngine;

namespace CreatureControllers
{
    public class UnitController : AiController
    {
        private Vector2? _moveCommandTarget;
        private Creature _target;
        private IInteractable _interactionTarget;

        private const bool MoveOnAttackCooldown = false; // TODO: This should be a configurable property of the weapon

        public void SetMoveTarget(Vector2 target)
        {
            _moveCommandTarget = target;
            _target = null;
        }

        public void SetTarget(Entity target)
        {
            switch (target)
            {
                case Creature creature:
                    _target = creature;
                    break;
                case IInteractable interactable:
                    _interactionTarget = interactable;
                    break;
            }
        }
        

        private void Update()
        {
            Creature.SetMovement(Vector2.zero);
            
            if(_interactionTarget != null)
            {
                HandleInteraction();
                return;
            }
            
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
                HandleAttack();
            }
        }

        private void HandleInteraction()
        {
            if (!_interactionTarget.CanInteract(Creature))
            {
                _interactionTarget = null;
                return;
            }

            if(Vector2.Distance(Creature.transform.position, _interactionTarget.Position) < Creature.InteractionRange)
            {
                var interaction = Creature.Interact(_interactionTarget);
                
                if(interaction.Status == InteractionStatus.Created)
                {
                    interaction.Completed += () =>
                    {
                        _interactionTarget = null;
                    };
                    
                    interaction.Canceled += () =>
                    {
                        _interactionTarget = null;
                    };
                }
                
                return;
            }
            else
            {
                PerformMovementTowardsPosition(_interactionTarget.Position);
            }
        }

        private void HandleAttack()
        {
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

            if (Creature.Weapon.GetOnCooldown(attackContext) && !Creature.Weapon.AllowToMoveOnCooldown)
            {
                Creature.SetMovement(Vector2.zero); // Stay still during cooldown if configured
                return;
            }

            if (Creature.Weapon.NeedsLineOfSight && !CanSee(_target))
            {
                PerformMovementTowardsTarget(_target);
                return;
            }

            if (Vector2.Distance(Creature.transform.position, _target.transform.position) < Creature.Weapon.Range)
            {
                PerformAttack(attackContext);
                return;
            }

            PerformMovementTowardsTarget(_target);
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