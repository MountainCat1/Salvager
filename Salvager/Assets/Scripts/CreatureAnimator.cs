using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class CreatureAnimator : MonoBehaviour
{
    [field: SerializeField] public Creature Creature { get; private set; }

    protected Animator _anim;
    protected SpriteRenderer _spriteRenderer;

    /// <summary>
    /// Grabs references to Animator and SpriteRenderer.
    /// </summary>
    protected virtual void Awake()
    {
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Subscribes to the Creature.Moved event.
    /// </summary>
    protected virtual void Start()
    {
        if (Creature != null)
        {
            Creature.Moved += OnMove;
        }
        else
        {
            Debug.LogWarning($"No Creature assigned to {name}!");
        }
    }

    /// <summary>
    /// Called whenever the creature moves (and is hooked to Creature.Moved).
    /// Flips the sprite by default.
    /// </summary>
    /// <param name="move">Movement vector</param>
    protected virtual void OnMove(Vector2 move)
    {
        // Flip sprite if moving left
        if(move.x == 0)
            return;
        
        _spriteRenderer.flipX = move.x > 0;
    }
}   