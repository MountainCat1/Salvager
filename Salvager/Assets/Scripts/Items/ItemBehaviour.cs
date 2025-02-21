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
        [field: SerializeField] public string Description { get; set; }
        [field: SerializeField] public float BaseCost { get; set; }

        protected ItemData ItemData;
        
        public virtual bool Stackable => true;
        public bool IsOriginal => Original is null;
        
        public ItemBehaviour Original { get; set; }

        public int Count
        {
            get => ItemData.Count;
            set => ItemData.Count = value;
        }

        public Inventory Inventory { get; set; } = null;

        private void Awake()
        {
            var dummyItemData = new ItemData
            {
                Identifier = "dummy item data DO NOT EVER FUCKING USE",
                Count = 1,
                Stackable = Stackable
            };
            
            SetData(dummyItemData); // uses it :3c
        }

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
            ItemData = itemData;
        }
        
        public virtual ItemData GetData()
        {
            return ItemData;
        }
    }
}