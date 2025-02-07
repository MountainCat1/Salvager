using System.Linq;
using UI;
using UnityEngine;

namespace CreatureControllers
{
    public class UnitController : AiController
    {
        private Vector2? _moveCommandTarget;
        private Creature _target;
        private IInteractable _interactionTarget;
        private Interaction _interaction;

        private const bool MoveOnAttackCooldown = false; // TODO: This should be a configurable property of the weapon

        // Public Methods
        public void SetMoveTarget(Vector2 target)
        {
            _moveCommandTarget = target;
            _target = null;
            _interactionTarget = null;
            _interaction?.Cancel();
        }

        public void SetTarget(Entity target)
        {
            _interaction?.Cancel();

            switch (target)
            {
                case Creature creature:
                    _target = creature;
                    break;
                case IInteractable interactable:
                    if (interactable.IsInteractable)
                        _interactionTarget = interactable;
                    break;
                default:
                    _target = null;
                    _interactionTarget = null;
                    Debug.LogWarning($"Invalid target type {target.GetType()}");
                    break;
            }
        }


        // Unity Methods
        private void Update()
        {
            if (_interactionTarget != null)
            {
                HandleInteraction();
                return;
            }

            if (_moveCommandTarget.HasValue)
            {
                HandleMovementToTarget();
                return;
            }

            if (Creature.Weapon)
            {
                Creature.SetMovement(Vector2.zero);
                return;
            }

            if (_target == null)
            {
                _target = GetNewTarget();
            }

            if (_target != null)
            {
                HandleAttack();
                return;
            }

            Creature.SetMovement(Vector2.zero);
        }


        // Private Methods
        private void HandleInteraction()
        {
            if (!_interactionTarget.CanInteract(Creature))
            {
                _interactionTarget = null;
                return;
            }

            if (Vector2.Distance(Creature.transform.position, _interactionTarget.Position) < Creature.InteractionRange)
            {
                Creature.SetMovement(Vector2.zero);
                InvokePathChanged(Enumerable.Empty<Vector2>());

                var interaction = Creature.Interact(_interactionTarget);

                if (interaction.Status == InteractionStatus.Created)
                {
                    _moveCommandTarget = null;

                    interaction.Completed += () => { _interactionTarget = null; };

                    interaction.Canceled += () => { _interactionTarget = null; };
                }

                Creature.SetMovement(Vector2.zero);
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
                InvokePathChanged(Enumerable.Empty<Vector2>()); // Clear the path
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

            if (Creature.Weapon.NeedsLineOfSight)
            {
                // TODO: we replaced those two cases aka "not shooting through allies", and "moving when cannot see", to avoid a weird behaviour
                // when creatures get closer when in a line and the enemy gets pushed so even though they wont attack they come closer
                // and break the formation
                
                if (!Creature.Weapon.ShootThroughAllies)
                {
                    // Stay still if there are friendly creatures in the line of sight
                    var creaturesInLineOfFire = GetCreatureInLine(_target.transform.position, Creature.Weapon.Range);
                    if (creaturesInLineOfFire.Any(x => x.GetAttitudeTowards(Creature) == Attitude.Friendly))
                    {
                        Creature.SetMovement(Vector2.zero);
                        return;
                    }
                }
                
                // Move towards the target if it is not in line of sight
                if (!CanSee(_target))
                {
                    PerformMovementTowardsTarget(_target);
                    return;
                }
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
            Creature.Weapon.ContinuousAttack(context);
        }
    }
}