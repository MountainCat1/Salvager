using System;
using Godot;

namespace Items;

public class ItemUseContext
{
    public Creature Creature { get; init; }
}

public partial class Item : Node2D
{
    public event Action<Item> Used;
        
    [Export] public Texture2D Icon { get; set; }
    [Export] public string Name { get; set; }
    [Export] public float Weight { get; set; } = 1f;

    public string GetIdentifier()
    {
        return $"{Name}";
    }
        
    public virtual void Use(ItemUseContext ctx)
    {
        var creature = ctx.Creature;
            
        Used?.Invoke(this);
         
        GD.Print($"{creature} using item " + Name);
    }
     
    public virtual void Pickup(ItemUseContext ctx)
    {
        var creature = ctx.Creature;

        GD.Print($"{creature} picked up item " + Name);
    }
}
