using Data;
using Managers;
using Zenject;

namespace Installer
{
    public class MenuInstaller : MonoInstaller<MenuInstaller>
    {
        public override void InstallBindings()
        {
            // Bindings

            Container.Bind<IItemManager>().To<ItemManager>().FromComponentsInHierarchy().AsSingle().NonLazy();
            Container.Bind<IDataManager>().To<DataManager>().FromNew().AsSingle().NonLazy();
        }
    }
}