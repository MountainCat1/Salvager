using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using Microsoft.Extensions.DependencyInjection;
using Salvager.Scripts.Services;

namespace Salvager.Scripts;

public class InjectAttribute : Attribute
{
}

public partial class GameSceneContext : Node
{
	// This DI Container is specific to THIS scene.
	public ServiceProvider? Container { get; private set; }

	public override void _Ready()
	{
		var services = new ServiceCollection();

		// Some di registrations
		services.AddSingleton<HelloWorldService>();
		
		Container = services.BuildServiceProvider();

		GD.Print("SceneContext DI container is initialized!");
		
		InstanceMethod();
	}

	private void InstanceMethod()
	{
		// Get ALL nodes in the scene
		var nodes = GetAllChildren(GetTree().Root);

		foreach (Node node in nodes)
		{
			// Get all fields with InjectAttribute
			var fields = node.GetType()
				.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
				.Where(f => f.GetCustomAttributes(typeof(InjectAttribute), false).Any());

			foreach (var field in fields)
			{
				// Inject dependencies using the service container
				var service = Container?.GetService(field.FieldType);
				if (service != null)
				{
					field.SetValue(node, service);
				}
			}

			// Invoke the "Start" method if it exists
			var startMethod = node.GetType().GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			startMethod?.Invoke(node, null);
		}
	}
	
	public static List<Node> GetAllChildren(Node inNode, List<Node> array = null)
	{
		if (array == null)
			array = new List<Node>();

		array.Add(inNode);

		foreach (Node child in inNode.GetChildren())
		{
			GetAllChildren(child, array);
		}

		return array;
	}
}