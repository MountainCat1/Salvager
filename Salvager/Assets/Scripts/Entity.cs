using System;
using Components;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public event Action<Vector2> Moved;

    // Components
    public MovementComponent Movement { get; private set; }
    public HealthComponent Health { get; private set; }

    protected Transform RootTransform;

    protected virtual void Awake()
    {
        RootTransform = transform;

        // Initialize MovementComponent
        Movement = GetComponent<MovementComponent>();
        if (Movement)
        {
            Movement.Moved += OnMoved;
        }

        // Initialize HealthComponent
        Health = GetComponent<HealthComponent>();
        if (Health)
        {
            Health.Death += HandleDeath;
        }
    }

    protected virtual void Update()
    {
    }

    public void SetMovement(Vector2 direction)
    {
        Movement.SetMovement(direction);
    }

    public void Push(Vector2 push)
    {
        Movement.Push(push);
    }

    public void Damage(HitContext ctx)
    {
        Health.Damage(ctx);
    }

    public void Heal(int healAmount)
    {
        Health.Heal(healAmount);
    }

    private void OnMoved(Vector2 velocity)
    {
        Moved?.Invoke(velocity);
    }

    private void HandleDeath(DeathContext context)
    {
        // Additional death-related logic for Entity
        Destroy(gameObject);
    }
}