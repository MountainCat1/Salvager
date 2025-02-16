using UnityEngine;

namespace Items
{
    public class HealthPotion : ItemBehaviour
    {
        [SerializeField] private int healAmount;
        
        public override void Use(ItemUseContext ctx)
        {
            base.Use(ctx);

            ctx.Creature.Heal(healAmount);

            ctx.Creature.Inventory.DeleteItem(this);
        }
    }
}