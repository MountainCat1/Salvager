using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
public class Creature : MonoBehaviour
{
    // Events
    public event Action<DeathContext> Death;
    public event Action<HitContext> Hit;
    public event Action WeaponChanged;

    // Injected Dependencies (using Zenject)
    [Inject] private ITeamManager _teamManager;
    [Inject] private DiContainer _diContainer;
    [Inject] private ICreatureManager _creatureManager;

    // Public Variables
    public Rigidbody2D Rigidbody2D => _rigidbody2D;
    public Inventory Inventory { get; private set; }
    public IReadonlyRangedValue Health => health;
    public ILevelSystem LevelSystem => _levelSystem;

    // Serialized Private Variables
    [field: Header("Movement")]
    [field: SerializeField]
    public float Drag { get; private set; }

    [field: SerializeField] public float BaseSpeed { get; set; }

    public Weapon Weapon
    {
        get => weapon;
        private set
        {
            weapon = value;
            WeaponChanged?.Invoke();
        }
    }
    [field: SerializeField] private Weapon weapon;

    [field: Header("Stats")] [field: SerializeField]
    private RangedValue health;

    [field: SerializeField] public float SightRange { get; private set; } = 13f;
    [field: SerializeField] public int XpAmount { get; private set; }
    [field: SerializeField] private Teams team;
    public float Speed => GetSpeed();

    // Private Variables
    private readonly LevelSystem _levelSystem = new();

    private Transform _rootTransform;
    private Rigidbody2D _rigidbody2D;
    private Vector2 _moveDirection;
    private Vector2 _momentum;
    private Creature _lastAttackedBy = null;

    private const float MomentumLoss = 2f;

    // Properties

    // Unity Callbacks
    private void Awake()
    {
        _rootTransform = transform;
        _rigidbody2D = GetComponent<Rigidbody2D>();

        health.ValueChanged += OnHealthChanged;
        health.CurrentValue = health.MaxValue;

        Inventory = new Inventory(_rootTransform);
        _diContainer.Inject(Inventory);
    }

    private void FixedUpdate()
    {
        UpdateVelocity();
    }

    // Public Methods
    public void SetMovement(Vector2 direction)
    {
        _moveDirection = direction.normalized;
    }

    public Attitude GetAttitudeTowards(Creature other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        if (other == this)
            return Attitude.Friendly;

        return _teamManager.GetAttitude(team, other.team);
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

    public void Heal(int healAmount)
    {
        health.CurrentValue += healAmount;
    }

    public static bool IsCreature(GameObject go)
    {
        return go.CompareTag("Player") || go.CompareTag("Creature");
    }

    public void StartUsingWeapon(Weapon weaponItem)
    {
        Weapon = weaponItem;
    }

    public void UseItem(ItemBehaviour item)
    {
        item.Use(new ItemUseContext()
        {
            Creature = this
        });
    }

    public void AwardXp(int amount)
    {
        _levelSystem.AddXp(amount);
    }


    // Virtual Methods

    // Abstract Methods

    // Private Methods
    private float GetSpeed()
    {
        return BaseSpeed + _levelSystem.CharacteristicsLevels[Characteristics.Dexterity] *
            CharacteristicsConsts.SpeedAdditiveMultiplierPerDexterity;
    }

    private void UpdateVelocity()
    {
        _momentum -= _momentum * (MomentumLoss * Time.fixedDeltaTime);
        if (_momentum.magnitude < 0.1f)
            _momentum = Vector2.zero;

        var change = Vector2.MoveTowards(_rigidbody2D.velocity, _moveDirection * Speed + _momentum,
            Drag * Time.fixedDeltaTime);
        _rigidbody2D.velocity = change;
    }

    public void Push(Vector2 push)
    {
        _momentum = push;
    }

    private void InvokeDeath()
    {
        var deathContext = new DeathContext()
        {
            Killer = _lastAttackedBy,
            Creature = this
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


    // Event Handlers

    protected virtual void OnHealthChanged()
    {
        if (health.CurrentValue <= health.MinValue)
            InvokeDeath();
    }
}

public struct DeathContext
{
    public Creature Killer { get; set; }
    public Creature Creature { get; set; }
}