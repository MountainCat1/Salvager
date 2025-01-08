using System;
using Godot;

namespace Items;

public class ItemUseContext
{
    public Creature Creature { get; }

    public ItemUseContext(Creature creature)
    {
        Creature = creature;
    }
}

public partial class Item : Node2D
{
    public event Action<Item>? Used;
        
    [Export] public Texture2D Icon { get; set; } = null!;
    [Export] public string CreatureName { get; set; } = null!;
    [Export] public float Weight { get; set; } = 1f;

    public string GetIdentifier()
    {
        return $"{CreatureName}";
    }
        
    public virtual void Use(ItemUseContext ctx)
    {
        var creature = ctx.Creature;
            
        Used?.Invoke(this);
         
        GD.Print($"{creature} using item " + CreatureName);
    }
     
    public virtual void Pickup(ItemUseContext ctx)
    {
        var creature = ctx.Creature;

        GD.Print($"{creature} picked up item " + CreatureName);
    }
}
