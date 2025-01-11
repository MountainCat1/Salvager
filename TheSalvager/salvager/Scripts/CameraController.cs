using System.Diagnostics;
using Godot;

public partial class CameraController : Node2D
{
    [Export] private Node2D? _target; // The target the camera follows
    [Export] private float _followSpeed = 5f; // Speed at which the camera follows the target
    [Export] private float _moveSpeed = 200f; // Speed for manual movement
    [Export] private bool _allowManualControl = true; // Allow manual camera movement with keyboard

    private Camera2D _camera2D;

    public override void _Ready()
    {
        base._Ready();
        _camera2D = GetNode<Camera2D>("CameraShake/Camera2D");
    }

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
        Debug.Assert(_target != null, "Target is null!");
        
        // Smoothly move the camera towards the target position
        GlobalPosition = GlobalPosition.Lerp(_target.GlobalPosition, (float)delta * _followSpeed);
    }

    private void HandleManualControl(double delta)
    {
        Vector2 direction = Vector2.Zero;

        if (Input.IsActionPressed("move_left"))
            direction.X -= 1;
        if (Input.IsActionPressed("move_right"))
            direction.X += 1;
        if (Input.IsActionPressed("move_up"))
            direction.Y -= 1;
        if (Input.IsActionPressed("move_down"))
            direction.Y += 1;

        if (direction != Vector2.Zero)
        {
            GlobalPosition += direction.Normalized() * _moveSpeed * (float)delta;
        }
        
        // Scrolling
        if (Input.IsActionJustReleased("zoom_in"))
        {
            _camera2D.Zoom += new Vector2(0.1f, 0.1f);
        }
        if (Input.IsActionJustReleased("zoom_out"))
        {
            _camera2D.Zoom -= new Vector2(0.1f, 0.1f);
        }
    }
    
}