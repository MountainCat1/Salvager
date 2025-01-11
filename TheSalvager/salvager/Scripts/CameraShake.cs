using Godot;

public partial class CameraShake : Node2D
{
    [Export] public float ShakeMultiplier = 15.0f; 
    [Export] public float ShakeDecreaseMultiplier = 50.0f; 
    
    private float currentShakeAmount = 0.0f; // Accumulated shake amount
    private Vector2 originalPosition;

    public override void _Ready()
    {
        // Store the original position of the node
        originalPosition = Position;
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
            currentShakeAmount -= floatDelta * currentShakeAmount * currentShakeAmount * ShakeDecreaseMultiplier;

            // Stop shaking when the shake amount is small enough
            if (currentShakeAmount < 0.01f)
            {
                currentShakeAmount = 0.0f;
                Position = originalPosition;
            }
        }
    }

    public void Shake(float amount)
    {
        // Accumulate the shake amount
        currentShakeAmount += amount * ShakeMultiplier;
    }
}
