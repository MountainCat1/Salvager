namespace Salvager.Scripts;

using Godot;

public partial class CameraController : Camera2D
{
    [Export] private Node2D _target; // The target the camera follows
    [Export] private float _followSpeed = 5f; // Speed at which the camera follows the target
    [Export] private float _moveSpeed = 200f; // Speed for manual movement
    [Export] private bool _allowManualControl = true; // Allow manual camera movement with keyboard

    public override void _Process(double delta)
    {
        if (_target != null)
        {
            FollowTarget(delta);
        }

        if (_allowManualControl)
        {
            HandleManualControl(delta);
        }
    }

    private void FollowTarget(double delta)
    {
        // Smoothly move the camera towards the target position
        GlobalPosition = GlobalPosition.Lerp(_target.GlobalPosition, (float)delta * _followSpeed);
    }

    private void HandleManualControl(double delta)
    {
        Vector2 direction = Vector2.Zero;

        if (Input.IsActionPressed("ui_left"))
            direction.X -= 1;
        if (Input.IsActionPressed("ui_right"))
            direction.X += 1;
        if (Input.IsActionPressed("ui_up"))
            direction.Y -= 1;
        if (Input.IsActionPressed("ui_down"))
            direction.Y += 1;

        if (direction != Vector2.Zero)
        {
            GlobalPosition += direction.Normalized() * _moveSpeed * (float)delta;
        }
    }
}
