using Godot;

public partial class Creature : CharacterBody2D
{
    [Export] private float Speed = 300f;
    [Export] private float Accel = 7f;

    private NavigationAgent2D _nav;
    private Vector2 _velocity = Vector2.Zero;

    public override void _Ready()
    {
        _nav = GetNode<NavigationAgent2D>("NavigationAgent2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        // Set the target position to the current mouse position
        _nav.TargetPosition = GetGlobalMousePosition();

        // Calculate the next direction based on the NavigationAgent2D's pathfinding
        Vector2 direction = (_nav.GetNextPathPosition() - GlobalPosition).Normalized();

        // Smoothly adjust the velocity towards the target
        _velocity = _velocity.Lerp(direction * Speed, Accel * (float)delta);

        Velocity = _velocity;
        
        // Move the character
        MoveAndSlide();
    }
}