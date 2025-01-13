using System;
using Godot;
using Services;

public partial class CameraShake : Node2D
{
    [Inject] private CameraController _cameraController = null!;
    
    [Export] public float ShakeMultiplier = 15.0f;
    [Export] public float ShakeDecreaseMultiplier = 50.0f;

    private float currentShakeAmount = 0.0f; // Accumulated shake amount
    private Vector2 originalPosition;
    private Camera2D _camera2D;

    public override void _Ready()
    {
        // Store the original position of the node
        originalPosition = Position;
        _camera2D = GetNode<Camera2D>("Camera2D") ?? throw new NullReferenceException();
    }

    public override void _Process(double delta)
    {
        if (currentShakeAmount > 0)
        {
            // Apply a random offset to create the shaking effect
            Position = originalPosition + new Vector2(
                (float)GD.RandRange(-currentShakeAmount, currentShakeAmount),
                (float)GD.RandRange(-currentShakeAmount, currentShakeAmount)
            );

            // Gradually reduce the shake amount over time
            var floatDelta = (float)delta;
            currentShakeAmount -= floatDelta * (currentShakeAmount * currentShakeAmount + 1f) * ShakeDecreaseMultiplier;

            // Stop shaking when the shake amount is small enough
            if (currentShakeAmount < 0.01f)
            {
                currentShakeAmount = 0.0f;
                Position = originalPosition;
            }
        }
    }

    public void Shake(float amount, Vector2 globalPosition)
    {
        // Example slope constants: change them as needed
        const float nearDistance = 200f;    // distance at which shake multiplier is 1
        const float farDistance  = 350f;   // distance at which shake multiplier is 0

        Vector2 cameraPosition = _cameraController.GlobalPosition;
        float distance = globalPosition.DistanceTo(cameraPosition);
    
        // Compute the distance-based multiplier [0..1]
        //  - If distance <= nearDistance => 1
        //  - If distance >= farDistance  => 0
        //  - Else interpolate linearly between
        float t = 1f - ((distance - nearDistance) / (farDistance - nearDistance));
        float distanceMultiplier = Mathf.Clamp(t, 0f, 1f);

        // Accumulate the shake amount
        currentShakeAmount += amount * ShakeMultiplier * distanceMultiplier;
    }
}