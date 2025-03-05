using Data;
using LevelSelector.Managers;
using Managers;
using Managers.LevelSelector;
using UI;
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
            Container.Bind<IItemDescriptionManager>().To<ItemDescriptionManager>().FromNew().AsSingle();
            Container.Bind<IUpgradeManager>().To<UpgradeManager>().FromComponentsInHierarchy().AsSingle().NonLazy();
            Container.Bind<ICrewManager>().To<CrewManager>().FromComponentsInHierarchy().AsSingle().NonLazy();
            Container.Bind<IShopGenerator>().To<ShopGenerator>().FromComponentsInHierarchy().AsSingle().NonLazy();
            Container.Bind<ILocationGenerator>().To<LocationGenerator>().FromComponentsInHierarchy().AsSingle().NonLazy();
            Container.Bind<IRegionGenerator>().To<RegionGenerator>().FromComponentsInHierarchy().AsSingle().NonLazy();
            Container.Bind<IRegionManager>().To<RegionManager>().FromComponentsInHierarchy().AsSingle().NonLazy();
            Container.Bind<ITravelManager>().To<TravelManager>().FromComponentsInHierarchy().AsSingle().NonLazy();
            Container.Bind<ILocationInteractionManager>().To<LocationInteractionManager>().FromComponentsInHierarchy().AsSingle().NonLazy();
            Container.Bind<IGameEventManager>().To<GameEventManager>().FromComponentsInHierarchy().AsSingle().NonLazy();
            
            // UI
            Container.Bind<IPanelManagerUI>().To<PanelManagerUI>().FromComponentsInHierarchy().AsSingle();
            Container.Bind<IFloatingTextService>().To<FloatingTextService>().FromComponentsInHierarchy().AsSingle();
        }        
    }
}