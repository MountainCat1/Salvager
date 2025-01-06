using Godot;

public partial class CreatureController : Node2D
{
    [Export] public Creature Creature { get; private set; }

    protected bool CanSee(Creature target)
    {
        // Calculate the direction and distance to the target
        var distance = Position.DistanceTo(target.Position);

        // If the target is too far away, we can't see it
        if (distance > Creature.SightRange)
            return false;

        // Perform a raycast to check for obstacles
        var spaceState = GetWorld2D().DirectSpaceState;

        // Define the starting point and endpoint of the ray
        var startPoint = Position;
        var endPoint = target.Position;

        // Perform the raycast
        var result = spaceState.IntersectRay(new PhysicsRayQueryParameters2D()
        {
            From = startPoint,
            To = endPoint,
        }); // TODO - Add collision mask and exclude self, also no clue if this works
        

        // If the result is null, there was no obstacle in the way
        return result == null;
    }
}