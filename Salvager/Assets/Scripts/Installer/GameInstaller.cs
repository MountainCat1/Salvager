using Data;
using Managers;
using Managers.Visual;
using Services.MapGenerators.GenerationSteps;
using UI;
using Zenject;

public class GameInstaller : MonoInstaller<GameInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<IDataManager>().To<DataManager>().FromNew().AsSingle().NonLazy();
        
        Container.Bind<IItemManager>().To<ItemManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<ISignalManager>().To<SignalManager>().FromNew().AsSingle().NonLazy();
        Container.Bind<ISpawnerManager>().To<SpawnerManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<IPoolingManager>().To<PoolingManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<IProjectileManager>().To<ProjectileManager>().FromNew().AsSingle().NonLazy();
        Container.Bind<IDataResolver>().To<DataResolver>().FromComponentsInHierarchy().AsSingle();
        // Container.Bind<IInputManager>().To<InputManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<IInputMapper>().To<InputMapper>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<ITimeManager>().To<TimeManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<ISelectionManager>().To<SelectionManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<IPathfinding>().To<CachedPathfinding>().FromInstance(new CachedPathfinding(FindObjectOfType<OldPathfinding>(), FindObjectOfType<GridGenerator>())).AsSingle();
        Container.Bind<IFlagManager>().To<FlagManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<ISoundPlayer>().To<SoundPlayer>().AsSingle().NonLazy();
        Container.Bind<ITeamManager>().To<TeamManager>().FromComponentsInHierarchy().AsSingle();
        // Container.Bind<IPopupManager>().To<PopupManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<ICreatureManager>().To<CreatureManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<ICreatureEventProducer>().To<CreatureEventProducer>().FromNew().AsSingle().NonLazy();
        Container.Bind<IRoomDecorator>().To<RoomDecorator>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<IMapGenerator>().To<StepDungeonGenerator>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<ICameraController>().To<CameraController>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<ICameraShakeService>().To<CameraShakeService>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<IVictoryConditionManager>().To<VictoryConditionManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<IEnemySpawner>().To<EnemySpawner>().FromComponentsInHierarchy().AsSingle();

        Container.Bind<IFloatingTextManager>().To<FloatingTextManager>()
            .FromInstance(FindObjectOfType<FloatingTextManager>());
        
    }
}
