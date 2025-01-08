using System.Diagnostics;
using Godot;
using Items;
using Services;
using Services.Abstractions;

namespace CreatureControllers;

public partial class UnitController : AiController
{
    private Vector2? _moveCommandTarget;
    private Creature? _target;
    private IInteractable? _interactionTarget;

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


    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        Creature.NavigationAgent.SetTargetPosition(Position);
        
        if (_interactionTarget != null)
        {
            // HandleInteraction();
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
            HandleAttackOrMovementToTarget(_target);
            return;
        }
    }

    // private void HandleInteraction()
    // {
    //     if (!_interactionTarget.CanInteract(Creature))
    //     {
    //         _interactionTarget = null;
    //         return;
    //     }
    //
    //     if (Vector2.Distance(Creature.transform.position, _interactionTarget.Position) < Creature.InteractionRange)
    //     {
    //         var interaction = Creature.Interact(_interactionTarget);
    //
    //         if (interaction.Status == InteractionStatus.Created)
    //         {
    //             interaction.Completed += () => { _interactionTarget = null; };
    //
    //             interaction.Canceled += () => { _interactionTarget = null; };
    //         }
    //
    //         return;
    //     }
    //     else
    //     {
    //         SetMovementTarget(_interactionTarget.Position);
    //     }
    // }


    private void HandleMovementToTarget()
    {
        Debug.Assert(_moveCommandTarget != null, nameof(_moveCommandTarget) + " != null");
        
        if (Creature.Position.DistanceTo(_moveCommandTarget.Value) > 5f)
        {
            Creature.NavigationAgent.SetTargetPosition(_moveCommandTarget.Value);
            Creature.MoveAndSlide();
        }
        else
        {
            _moveCommandTarget = null; // Reached the destination
        }
    }

    private void HandleAttackOrMovementToTarget(Creature target)
    {
        var attackContext = CreateAttackContext();
    
        if(Creature.Weapon == null)
        {
            GD.PushWarning("Creature has no weapon equipped");
            
            return;
        }
        
        if (Creature.Weapon.GetOnCooldown(attackContext) && !Creature.Weapon.AllowToMoveOnCooldown)
        {
            return;
        }
    
        if (Creature.Weapon.NeedsLineOfSight && !CanSee(target))
        {
            Creature.NavigationAgent.SetTargetPosition(target.Position);
            Creature.MoveAndSlide();
            
            return;
        }
    
        if ((Creature.Position.DistanceTo(target.Position) < Creature.Weapon.Range))
        {
            PerformAttack(attackContext);
            return;
        }
    
        Creature.NavigationAgent.SetTargetPosition(target.Position);
        Creature.MoveAndSlide();
    }

    private AttackContext CreateAttackContext()
    {
        Debug.Assert(_target != null, nameof(_target) + " != null");
        
        return new AttackContext
        {
            Direction = (_target.Position - Creature.Position).Normalized(),
            Target = _target,
            Attacker = Creature
        };
    }


    private void PerformAttack(AttackContext context)
    {
        Creature.Weapon?.ContinuousAttack(context);
    }
}