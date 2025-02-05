using Data;
using Managers;
using Managers.LevelSelector;
using Zenject;

namespace Installer
{
    public class LevelSelectorInstaller : MonoInstaller<LevelSelectorInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IDataManager>().To<DataManager>().FromNew().AsSingle().NonLazy();
            Container.Bind<ICrewManager>().To<CrewManager>().FromComponentsInHierarchy().AsSingle().NonLazy();
            Container.Bind<IRegionGenerator>().To<RegionGenerator>().FromComponentsInHierarchy().AsSingle().NonLazy();
            Container.Bind<IRegionManager>().To<RegionManager>().FromComponentsInHierarchy().AsSingle().NonLazy();
        }        
    }
}