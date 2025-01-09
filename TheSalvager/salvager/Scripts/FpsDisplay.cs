
using Godot;

public partial class FpsDisplay : Node2D
{
    // set window name
    public override void _Process(double delta)
    {
        base._Process(delta);
        
        var fps = Engine.GetFramesPerSecond();
        DisplayServer.WindowSetTitle($"FPS: {fps}"); 
    }
}