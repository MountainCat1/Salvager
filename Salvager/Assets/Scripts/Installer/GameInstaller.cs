using Managers;
using Zenject;

public class GameInstaller : MonoInstaller<GameInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<IGameDataManager>().To<GameDataManager>().FromNew().AsSingle();
        Container.Bind<ISignalManager>().To<SignalManager>().FromNew().AsSingle().NonLazy();
        Container.Bind<IInputManager>().To<InputManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<IInputMapper>().To<InputMapper>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<ITimeManager>().To<TimeManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<ISelectionManager>().To<SelectionManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<IWinManager>().To<WinManager>().FromNew().AsSingle();
        Container.Bind<IPathfinding>().To<Pathfinding>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<IFlagManager>().To<FlagManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<ISoundPlayer>().To<SoundPlayer>().AsSingle().NonLazy();
        Container.Bind<ITeamManager>().To<TeamManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<IPopupManager>().To<PopupManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<ICreatureManager>().To<CreatureManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<IPhaseManager>().To<PhaseManager>().FromComponentsInHierarchy().AsSingle();
    }
}
