using System;
using UnityEngine;

public class SimpleCreatureAnimator : CreatureAnimator
{
    [Header("Animation Clips to Override")] [SerializeField]
    private AnimationClip idle;

    [SerializeField] private AnimationClip walk;
    [SerializeField] private AnimationClip attack;

    /// <summary>
    /// In Awake, we load the base controller from Resources and override its clips.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        // 1. Load a base controller named "MyBaseCreatureController.controller" from a Resources folder
        var baseController = Resources.Load<RuntimeAnimatorController>("BaseCreatureAnimator");
        if (baseController == null)
        {
            Debug.LogError("Could not find 'BaseCreatureAnimator.controller' in a Resources folder!");
            return;
        }

        // 2. Create an AnimatorOverrideController from that base
        AnimatorOverrideController overrideController = new AnimatorOverrideController(baseController);

        // 3. Override "Idle", "Walk", and "Run" states with your custom clips
        overrideController["Idle"] = idle;
        overrideController["Walk"] = walk;
        overrideController["Attack"] = attack;

        // 4. Assign the override controller to the Animator
        _anim.runtimeAnimatorController = overrideController;
    }

    protected override void Start()
    {
        base.Start();

        if (Creature != null)
        {
            Creature.StateChanged += OnState;
            Creature.Attacked += OnAttack;
        }
        else
        {
            Debug.LogWarning($"No Creature assigned to {name}!");
        }
    }

    private void OnAttack(AttackContext ctx)
    {
        if(ctx.Direction.x != 0)
            _spriteRenderer.flipX = ctx.Direction.x > 0;
    }


    /// <summary>
    ///  OnMove is called whenever the creature moves (and is hooked to Creature.Moved).
    /// </summary>
    /// <param name="obj"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnState(CreatureState state)
    {
        switch (state)
        {
            case CreatureState.Idle:
                _anim.Play("Idle");
                break;
            case CreatureState.Moving:
                _anim.Play("Walk");
                break;
            case CreatureState.Attacking:
                _anim.Play("Attack");
                break;
            default:
                throw new NotImplementedException();
        }
    }
}