using System;
using System.Diagnostics;
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

public interface IDIContext
{
    ServiceProvider? Container { get; }
    public void Inject<T>(T injectionTarget);
    public void FullInject<T>(T injectionTarget) where T : Node;
}

public partial class DIContext : Node, IDIContext
{
    // This DI Container is specific to THIS scene.
    public ServiceProvider? Container { get; private set; }

    private bool _isInitialized = false;

    protected virtual void InstallBindings(ServiceCollection services)
    {
    }

    public override void _Ready()
    {
        GD.Print("Initializing SceneContext DI container...");

        var services = new ServiceCollection();

        InstallBindings(services);

        Container = services.BuildServiceProvider();


        try
        {
            InjectDependencies();
        }
        catch (Exception e)
        {
            GD.PrintErr($"ERROR ACCRUED WHILE INJECTING DEPENDENCIES: {e.Message}");
            GD.PrintErr($"{e.StackTrace}");
            throw;
        }

        GD.Print("SceneContext DI container setup!");
    }

    public override void _Process(double delta)
    {
        if (_isInitialized)
            return;

        _isInitialized = true;

        RunStartMethods();

        GD.Print("SceneContext run Start() methods!");
    }

    public void Inject<T>(T injectionTarget)
    {
        Debug.Assert(injectionTarget != null, nameof(injectionTarget) + " != null");

        var fields = injectionTarget.GetType()
            .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Where(f => f.GetCustomAttributes(typeof(InjectAttribute), false).Any());

        foreach (var field in fields)
        {
            // Inject dependencies using the service container
            var service = Container?.GetService(field.FieldType);
            if (service != null)
            {
                field.SetValue(injectionTarget, service);
            }
            else if (field.FieldType == typeof(IDIContext))
            {
                field.SetValue(injectionTarget, this);
            }
            else
            {
                throw new NullReferenceException(
                    $"Service not found for {field.FieldType.Name} in {injectionTarget.GetType().Name}");
            }
        }
    }

    public void FullInject<T>(T injectionTarget) where T : Node
    {
        var nodes = injectionTarget.GetWithAllChildren();

        foreach (var node in nodes)
        {
            Inject(node);
        }

        foreach (var node in nodes)
        {
            InvokeStartMethod(node);
        }
    }


    protected IServiceCollection AddSingletonFromTree<TInterface, TImplementation>(IServiceCollection services)
        where TImplementation : class,
        TInterface
        where TInterface : class
    {
        var nodes = this.GetAllChildren();
        TImplementation? implementation = null;

        foreach (var node in nodes)
        {
            if (node is TImplementation service)
            {
                implementation = service;
                break;
            }
        }

        if (implementation == null)
            throw new NullReferenceException($"{typeof(TImplementation).Name} not found in the scene");

        services.AddSingleton<TInterface, TImplementation>(x => implementation);

        return services;
    }

    protected IServiceCollection AddSingletonFromInstance<TInterface, TImplementation>(IServiceCollection services,
        TImplementation instance)
        where TImplementation : class,
        TInterface
        where TInterface : class
    {
        services.AddSingleton<TInterface, TImplementation>(x => instance);

        return services;
    }

    protected IServiceCollection AddSingletonFromInstanceInScene<TInterface, TImplementation>(
        IServiceCollection services)
        where TImplementation : Node, TInterface
        where TInterface : class
    {
        TImplementation instance = NodeUtilities.FindRequiredNodeOfType<TImplementation>(this);

        return services.AddSingleton<TInterface>(instance);
    }

    private void InjectDependencies()
    {
        // Get ALL nodes in the scene
        var nodes = this.GetTree().Root.GetAllChildren();

        foreach (Node node in nodes)
        {
            // Get all fields with InjectAttribute
            Inject(node);
        }
    }


    private void RunStartMethods()
    {
        // Get ALL nodes in the scene
        var nodes = this.GetTree().Root.GetAllChildren();

        foreach (Node node in nodes)
        {
            // Invoke the "Start" method if it exists
            InvokeStartMethod(node);
        }
    }

    private void InvokeStartMethod<T>(T target)
    {
        Debug.Assert(target != null, nameof(target) + " != null");

        var startMethod = target.GetType().GetMethod("Start",
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        startMethod?.Invoke(target, null);
    }
}