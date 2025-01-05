using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Godot;
using Microsoft.Extensions.DependencyInjection;
using Utilities;

namespace Services;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class InjectAttribute : Attribute
{
}

public partial class DIContext : Node
{
    // This DI Container is specific to THIS scene.
    public ServiceProvider? Container { get; private set; }

    protected virtual void InstallBindings(ServiceCollection services)
    {
    }

    public override void _Ready()
    {
        var services = new ServiceCollection();

        InstallBindings(services);

        Container = services.BuildServiceProvider();
        GD.Print("SceneContext DI container is initialized!");

        InstanceMethod();
    }


    protected IServiceCollection AddSingletonFromTree<TInterface, TImplementation>(IServiceCollection services)
        where TImplementation : class,
        TInterface
        where TInterface : class
    {
        var nodes = this.GetAllChildren();

        foreach (Node node in nodes)
        {
            if (node is TImplementation service)
            {
                services.AddSingleton<TInterface, TImplementation>(x => service);
            }
        }

        return services;
    }

    private void InstanceMethod()
    {
        // Get ALL nodes in the scene
        var nodes = this.GetAllChildren();

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
            var startMethod = node.GetType().GetMethod("Start",
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            startMethod?.Invoke(node, null);
        }
    }
}