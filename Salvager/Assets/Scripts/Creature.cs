﻿using System;
using Components;
using Items;
using Managers;
using UI;
using UnityEngine;
using Zenject;

public enum CreatureState
{
    Idle,
    Moving,
    Attacking,
    Utility
}

[RequireComponent(typeof(Rigidbody2D))]
public class Creature : Entity, IDamageable
{
    public static float ColliderSize => 0.5f;
    
    // Events
    public event Action WeaponChanged;
    public event Action<CreatureState> StateChanged;
    public event Action<Interaction> Interacted;
    public event Action<AttackContext> Attacked; 

    // Injected Dependencies (using Zenject)
    [Inject] private ITeamManager _teamManager;
    [Inject] private DiContainer _diContainer;
    [Inject] private ICreatureManager _creatureManager;
    [Inject] private IFloatingTextManager _floatingTextManager;

    // Public Variables

    public CreatureState State
    {
        get => _state;
        private set
        {
            if (_state == value)
                return;

            _state = value;
            StateChanged?.Invoke(_state);
        }
    }

    private CreatureState _state = CreatureState.Idle;

    // Serialized Private Variables
    [field: Header("References")] [field: SerializeField]
    private Transform inventoryRoot;

    [field: SerializeField] private Weapon weapon;

    public Weapon Weapon
    {
        get => weapon;
        private set
        {
            if(weapon != null)
                weapon.Attacked -= OnWeaponAttack;
            
            weapon = value;
            
            if(value != null)
                weapon.Attacked += OnWeaponAttack;
            
            WeaponChanged?.Invoke();
        }
    }

    [field: SerializeField] public float SightRange { get; private set; } = 13f;
    [field: SerializeField] public int XpAmount { get; private set; }

    [field: Header("Other")]
    [field: SerializeField]
    public Teams Team { get; private set; }

    [field: SerializeField] public float InteractionRange { get; private set; } = 1.5f;

    // Accessors
    public CreatureController Controller => _controller;
    public Inventory Inventory => _inventory;
    public ILevelSystem LevelSystem => _levelSystem;
    public event Action Disabled;

    // Private Referenes

    private CreatureController _controller;
    private readonly LevelSystem _levelSystem = new();
    private Inventory _inventory;

    // Unity Callbacks
    protected override void Awake()
    {
        base.Awake();

        if (weapon != null)
        {
            Weapon = weapon;
        }
        
        if (inventoryRoot == null)
        {
            inventoryRoot = new GameObject("Inventory").transform;
            inventoryRoot.SetParent(RootTransform);
            inventoryRoot.localPosition = Vector3.zero;
            Debug.LogWarning("Inventory root is not set, creating a new one", this);
        }
        
        _controller = GetComponent<CreatureController>();

        _inventory = new Inventory(inventoryRoot);
        _diContainer.Inject(Inventory);

        Health.Hit += OnHit;
    }
    
    protected override void Update()
    {
        base.Update();

        if (weapon is not null && weapon.IsOnCooldown)
            State = CreatureState.Attacking;
        else if (Movement.MoveDirection.magnitude > 0)
            State = CreatureState.Moving;
        else
            State = CreatureState.Idle;
    }

    private void OnDestroy()
    {
        StartUsingWeapon(null);
    }

    private void OnDisable()
    {
        Disabled?.Invoke();
    }

    // Public Methods
    public Attitude GetAttitudeTowards(Creature other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        if (other == this)
            return Attitude.Friendly;

        return _teamManager.GetAttitude(Team, other.Team);
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
    public void Initialize(CreatureData data)
    {
        name = data.Name;
        SightRange = data.SightRange;
        Inventory.SetData(data.Inventory);
    }

    public Interaction Interact(IInteractable interactionTarget)
    {
        var interaction = interactionTarget.Interact(this, Time.deltaTime);
        if (interaction.Status == InteractionStatus.Created)
        {
            _floatingTextManager.SpawnFloatingText(RootTransform.position, interaction.Message, FloatingTextType.Interaction);
        }

        Interacted?.Invoke(interaction);
        return interaction;
    }

    // Static Methods

    public static bool IsCreature(GameObject go)
    {
        return go.CompareTag("Player") || go.CompareTag("Creature");
    }

    // Virtual Methods

    // Abstract Methods

    // Private Methods
    
    private void OnWeaponAttack(AttackContext ctx)
    {
       Attacked?.Invoke(ctx);
    }
    
    private void OnHit(HitContext ctx)
    {
        if (ctx.Attacker is not null && ctx.Push.magnitude > 0)
            Push(ctx.Push);
    }
}

public struct DeathContext
{
    public Entity Killer { get; set; }
    public Entity KilledEntity { get; set; }
}