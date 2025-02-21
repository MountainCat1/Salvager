using Data;
using LevelSelector.Managers;
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
            Container.Bind<IDataResolver>().To<DataResolver>().FromComponentsInHierarchy().AsSingle().NonLazy();
            Container.Bind<ISoundPlayer>().To<SoundPlayer>().FromNew().AsSingle();
            Container.Bind<IItemManager>().To<ItemManager>().FromComponentsInHierarchy().AsSingle();
            Container.Bind<IUpgradeManager>().To<UpgradeManager>().FromComponentsInHierarchy().AsSingle().NonLazy();
            Container.Bind<ICrewManager>().To<CrewManager>().FromComponentsInHierarchy().AsSingle().NonLazy();
            Container.Bind<IShopGenerator>().To<ShopGenerator>().FromComponentsInHierarchy().AsSingle().NonLazy();
            Container.Bind<ILocationGenerator>().To<LocationGenerator>().FromComponentsInHierarchy().AsSingle().NonLazy();
            Container.Bind<IRegionGenerator>().To<RegionGenerator>().FromComponentsInHierarchy().AsSingle().NonLazy();
            Container.Bind<IRegionManager>().To<RegionManager>().FromComponentsInHierarchy().AsSingle().NonLazy();
            Container.Bind<ILocationInteractionManager>().To<LocationInteractionManager>().FromComponentsInHierarchy().AsSingle().NonLazy();
        }        
    }
}