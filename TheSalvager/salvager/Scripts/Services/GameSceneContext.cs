using Microsoft.Extensions.DependencyInjection;
using Services.Abstractions;
using Services.MapGenerators;

namespace Services;

public partial class GameSceneContext : DIContext
{
    protected override void InstallBindings(ServiceCollection services)
    {
        // Some di registrations
        AddSingletonFromTree<ICreatureManager, CreatureManager>(services);
        AddSingletonFromTree<ITeamManager, TeamManager>(services);
        AddSingletonFromTree<ISelectionManager, SelectionManager>(services);
        AddSingletonFromTree<ISoundPlayer, SoundPlayer>(services);
        AddSingletonFromTree<ISpawnerManager, SpawnerManager>(services);
        AddSingletonFromTree<IPoolingManager, PoolingManager>(services);
        AddSingletonFromTree<ICameraShakeService, CameraShakeService>(services);
        AddSingletonFromTree<IInputManager, InputManager>(services);

        AddSingletonFromInstanceInScene<CameraController, CameraController>(services);
        AddSingletonFromInstanceInScene<IRoomDecorator, RoomDecorator>(services);
        AddSingletonFromInstanceInScene<IMapGenerator, DungeonGenerator>(services);
    }
}