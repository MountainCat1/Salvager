using Godot;

namespace Services;

public partial class InputManager : Node2D
{
    public override void _Ready()
    {
        base._Ready();
        
        // on escape key press go back to main menu
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            GoBack();
        }   
    }

    private void GoBack()
    {
        GetTree().ChangeSceneToFile("res://Scenes/MenuUIScene.tscn");
    }
}