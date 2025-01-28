using System;
using System.Linq;
using Components;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public event Action<Vector2> Moved;

    [field: Header("Movement")]
    [field: SerializeField]
    public float Drag { get; private set; }

    [field: SerializeField] 
    public float BaseSpeed { get; set; }

    // Accessors
    public Rigidbody2D Rigidbody2D => _rigidbody2D;
    public CircleCollider2D MovementCollider => _movementCollider;
    protected Transform RootTransform => _rootTransform;
    protected Vector2 MoveDirection => _moveDirection;
    public float Speed => GetSpeed();
    public HealthComponent Health => _healthComponent;
    
    // Private References
    private Transform _rootTransform;
    private Rigidbody2D _rigidbody2D;
    private CircleCollider2D _movementCollider;
    private HealthComponent _healthComponent;

    // Private Variables
    private Vector2 _moveDirection;
    private Vector2 _momentum;

    private const float MomentumLoss = 2f;

    protected virtual void Awake()
    {
        _rootTransform = transform;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _movementCollider = transform.GetComponentsInChildren<CircleCollider2D>()
            .Single(x => x.gameObject.layer == LayerMask.NameToLayer("CreatureMovement"));

        // Find or attach HealthComponent
        _healthComponent = GetComponent<HealthComponent>();
        if (_healthComponent == null)
        {
            _healthComponent = gameObject.AddComponent<HealthComponent>();
        }

        _healthComponent.Death += HandleDeath;
    }

    protected virtual void Update()
    {
        UpdateVelocity();
    }

    public void SetMovement(Vector2 direction)
    {
        _moveDirection = direction.normalized;
    }

    public void Damage(HitContext ctx)
    {
        _healthComponent.Damage(ctx);
    }

    public void Heal(int healAmount)
    {
        _healthComponent.Heal(healAmount);
    }

    public void Push(Vector2 push)
    {
        _momentum = push;
    }

    private void UpdateVelocity()
    {
        _momentum -= _momentum * (MomentumLoss * Time.fixedDeltaTime);
        if (_momentum.magnitude < 0.1f)
            _momentum = Vector2.zero;

        var change = Vector2.MoveTowards(_rigidbody2D.velocity, _moveDirection * Speed + _momentum,
            Drag * Time.fixedDeltaTime);
        _rigidbody2D.velocity = change;

        Moved?.Invoke(change);
    }

    private void HandleDeath(DeathContext context)
    {
        // Additional death-related logic for Entity
        Destroy(gameObject);
    }

    protected virtual float GetSpeed()
    {
        return BaseSpeed;
    }
}
