using System;
using System.Linq;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public event Action<DeathContext> Death;
    public event Action<HitContext> Hit;
    public event Action<Vector2> Moved;
    
    [field: Header("Stats")] 
    [field: SerializeField] 
    protected RangedValue health;
    
    [field: Header("Movement")]
    [field: SerializeField]
    public float Drag { get; private set; }

    [field: SerializeField] 
    public float BaseSpeed { get; set; }

    
    // Accessors
    public IReadonlyRangedValue Health => health;
    public Rigidbody2D Rigidbody2D => _rigidbody2D;
    public CircleCollider2D MovementCollider => _movementCollider;
    protected Transform RootTransform => _rootTransform;
    protected Vector2 MoveDirection => _moveDirection;
    public float Speed => GetSpeed();
    
    // Private References
    private Transform _rootTransform;
    private Rigidbody2D _rigidbody2D;
    private CircleCollider2D _movementCollider;
    
    // Private Variables
    private Vector2 _moveDirection;
    private Vector2 _momentum;
    private Creature _lastAttackedBy = null;
    
    private const float MomentumLoss = 2f;

    protected virtual void Awake()
    {
        _rootTransform = transform;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _movementCollider = transform.GetComponentsInChildren<CircleCollider2D>().Single(x => x.gameObject.layer == LayerMask.NameToLayer("CreatureMovement"));
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
        ctx.ValidateAndLog();

        health.CurrentValue -= ctx.Damage;
        _lastAttackedBy = ctx.Attacker;

        if (ctx.Push.magnitude > 0)
            Push(ctx.Push);

        Hit?.Invoke(ctx);
    }
    
    public void Push(Vector2 push)
    {
        _momentum = push;
    }

    public void Heal(int healAmount)
    {
        health.CurrentValue += healAmount;
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

    private void InvokeDeath()
    {
        var deathContext = new DeathContext()
        {
            Killer = _lastAttackedBy,
            KilledEntity = this
        };

        try
        {
            Death?.Invoke(deathContext);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }

        Destroy(gameObject);
    }
    
    // Private Methods
    protected virtual float GetSpeed()
    {
        return BaseSpeed;
    }
    
    // Event Handlers

    protected virtual void OnHealthChanged()
    {
        if (health.CurrentValue <= health.MinValue)
            InvokeDeath();
    }

}