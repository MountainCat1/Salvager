using Godot;
using System;

public partial class LevelSelectUI : Control
{
	[Export] private PackedScene _levelScene = null!;
	
	[Export] private Button _startButton = null!;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_startButton.Pressed += StartLevel;
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void StartLevel()
	{
		GetTree().ChangeSceneToPacked(_levelScene);
	}

}
