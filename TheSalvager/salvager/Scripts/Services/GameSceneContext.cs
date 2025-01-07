using System;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.Abstractions;
using Utilities;


public partial class GameSceneContext : DIContext
{
    protected override void InstallBindings(ServiceCollection services)
    {
        // Some di registrations
        AddSingletonFromTree<ICreatureManager, CreatureManager>(services);
        AddSingletonFromTree<ISelectionManager, SelectionManager>(services);
        AddSingletonFromInstance<IMapGenerator, DungeonGenerator>(services, NodeUtilities.FindNodeOfType<DungeonGenerator>(this) 
                                                                            ?? throw new NullReferenceException("DungeonGenerator not found"));
    }
}