using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Items
{
    public abstract class ItemBehaviour : MonoBehaviour
    {
        public event Action<ItemBehaviour> Used;
        
        [field: SerializeField] public Sprite Icon { get; set; }
        [field: SerializeField] public string Name => ItemData.Name;
        [field: SerializeField] public float Weight => 696969f; // TODO: Implement weight
        [field: SerializeField] public string Description { get; set; }
        [field: SerializeField] public float BaseCost { get; set; }
        
        public virtual bool Stackable => true;
        
        public ItemBehaviour Original { get; set; }
        public int Count { get; set; } = 1;

        public Inventory Inventory { get; set; } = null;
        public bool IsPrefab => Original is null;

        public abstract ItemData ItemData { get; protected set; }

        public string GetIdentifier()
        {
            return $"{Name}";
        }
        
        public virtual void Use(ItemUseContext ctx)
        {
            var creature = ctx.Creature;
            if(creature == null)
            {
                Debug.LogError("Creature is null");
                return;
            }
            
            Used?.Invoke(this);
         
            Debug.Log($"{creature} using item " + Name);
        }
     
        public virtual void Pickup(ItemUseContext ctx)
        {
            var creature = ctx.Creature;
            if(creature == null)
            {
                Debug.LogError("Creature is null");
                return;
            }
         
            Debug.Log($"{creature} picked up item " + Name);
        }

        public virtual void SetData(ItemData itemData)
        {
            Count = itemData.Count;
            
            ItemData = itemData;
        }
    }
}