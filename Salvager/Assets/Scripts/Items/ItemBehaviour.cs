using System;
using UnityEngine;

namespace Items
{
    public class ItemBehaviour : MonoBehaviour
    {
        public event Action<ItemBehaviour> Used;
        
        [field: SerializeField] public Sprite Icon { get; set; }
        [field: SerializeField] public string Name { get; set; }

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