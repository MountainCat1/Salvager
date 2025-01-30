using System;
using Items;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class ItemPickup : InteractableObject
{
    [SerializeField] private ItemBehaviour itemBehaviour;

    public Rigidbody2D Rigidbody2D => _rigidbody2D;

    private Rigidbody2D _rigidbody2D;

    protected override void Awake()
    {
        base.Awake();

        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    protected override void OnInteractionComplete(Interaction interaction)
    {
        base.OnInteractionComplete(interaction);

        interaction.Creature.Inventory.AddItem(itemBehaviour);

        Destroy(gameObject);
    }

    public void SetItem(ItemBehaviour item)
    {
        if (item == null)
            throw new Exception("ItemBehaviour is not set in ItemPickup");
        
        if(item.Original == null)
            throw new Exception("ItemBehaviour.Original is not set in ItemPickup");
        
        itemBehaviour = item.Original;

        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = itemBehaviour.Icon;
    }
}