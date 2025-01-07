using System.Diagnostics;
using System.Linq;
using Abstractions;
using Godot;
using Items;
using Services;
using Services.Abstractions;

public partial class UnitController : AiController
{
    [Inject] private IMapGenerator _mapGenerator = null!;
    
    private Vector2? _moveCommandTarget;
    private Creature? _target;
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


    public override void _PhysicsProcess(double delta)
    {
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

        // if (_target == null)
        // {
        //     _target = GetNewTarget();
        // }
        //
        if (_target != null)
        {
            HandleAttack();
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
    }
    

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

    private void HandleAttackOrMovementToTarget()
    {
        var attackContext = CreateAttackContext();
    
        if (Creature.Weapon.GetOnCooldown(attackContext) && !Creature.Weapon.AllowToMoveOnCooldown)
        {
            return;
        }
    
        if (Creature.Weapon.NeedsLineOfSight && !CanSee(_target))
        {
            Creature.NavigationAgent.SetTargetPosition(Creature.Position);
            Creature.MoveAndSlide();
            
            return;
        }
    
        if ((Creature.Position.DistanceTo(_target.Position) < Creature.Weapon.Range))
        {
            PerformAttack(attackContext);
            return;
        }
    
        Creature.NavigationAgent.SetTargetPosition(Creature.Position);
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
        // TODO: Implement attack
        // Creature.Weapon.ContiniousAttack(context);
    }
}