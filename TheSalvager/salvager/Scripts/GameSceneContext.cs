using Godot;
using Microsoft.Extensions.DependencyInjection;

namespace Salvager.Scripts;

public partial class GameSceneContext : Node
{
	// This DI Container is specific to THIS scene.
	public ServiceProvider? Container { get; private set; }

	public override void _Ready()
	{
		var services = new ServiceCollection();

		// Some di registrations
		
		Container = services.BuildServiceProvider();

		GD.Print("SceneContext DI container is initialized!");
	}

	private void InstanceMethod()
	{
		foreach (Node node in GetTree().Root.GetChildren())
		{
			// IMPLEMENT
		}
	}
}