using Godot;
using Services;

namespace DefaultNamespace;

public partial class ShakeGenerator : Sprite2D
{
    [Inject] ICameraShakeService _cameraShakeService = null!;


    public override void _Process(double delta)
    {
        base._Process(delta);
        
        _cameraShakeService.Shake(0.5f, Position);
    }
}