using System;
using UnityEngine;

namespace Items
{
    public class ItemBehaviour : MonoBehaviour
    {
        public event Action<ItemBehaviour> Used;
        
        [field: SerializeField] public Sprite Icon { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public float Weight { get; set; } = 1f;

        public virtual bool Stackable => true;
        
        public ItemBehaviour Original { get; set; }
        public int Count { get; set; } = 1;

        public Inventory Inventory { get; set; } = null;

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
    }
}