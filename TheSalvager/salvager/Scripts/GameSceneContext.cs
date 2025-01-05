using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.Abstractions;
using Utilities;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class InjectAttribute : Attribute
{
}

public partial class GameSceneContext : DIContext
{
    protected override void InstallBindings(ServiceCollection services)
    {
        // Some di registrations
        services.AddSingleton<IHelloWorldService, HelloWorldService>();

        AddSingletonFromTree<ICreatureManager, CreatureManager>(services);
        AddSingletonFromTree<ISelectionManager, SelectionManager>(services);
    }
}