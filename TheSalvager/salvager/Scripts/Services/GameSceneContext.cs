using Microsoft.Extensions.DependencyInjection;
using Services.Abstractions;

namespace Services;

public partial class GameSceneContext : DIContext
{
    protected override void InstallBindings(ServiceCollection services)
    {
        // Some di registrations
        AddSingletonFromTree<ICreatureManager, CreatureManager>(services);
        AddSingletonFromTree<ISelectionManager, SelectionManager>(services);
        AddSingletonFromTree<ISoundPlayer, SoundPlayer>(services);
        
        AddSingletonFromInstanceInScene<IMapGenerator, DungeonGenerator>(services);
    }
}