using Godot;
using Utilities;

namespace Services;

public interface ICameraShakeService
{
    public void Shake(float amount);
}

public partial class CameraShakeService : Node2D, ICameraShakeService
{
    private CameraShake _cameraShake = null!;
    
    public override void _Ready()
    {
        base._Ready();

        _cameraShake = NodeUtilities.FindRequiredNodeOfType<CameraShake>(this);
    }

    public void Shake(float amount)
    {
        _cameraShake.Shake(amount);        
    }
}