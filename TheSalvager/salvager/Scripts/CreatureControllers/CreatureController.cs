using System;
using System.Linq;
using Godot;

public partial class CreatureController : Node2D
{
    [Export] public Creature Creature { get; private set; } = null!;

    public override void _Ready()
    {
        base._Ready();
        
        Creature.Controller = this;
    }

    protected bool CanSee(Creature target)
    {
        var distance = Creature.Position.DistanceTo(target.Position);
        if (distance > Creature.SightRange)
            return false;

        var spaceState = GetWorld2D().DirectSpaceState;
        var startPoint = Creature.Position;
        var endPoint = target.Position;

        var query = new PhysicsRayQueryParameters2D
        {
            From = startPoint,
            To = endPoint,
            CollisionMask = CollisionMasks.Walls
        };
        var result = spaceState.IntersectRay(query);

        if (result.Count > 0)
        {
            return false;
        }

        return true;
    }

}