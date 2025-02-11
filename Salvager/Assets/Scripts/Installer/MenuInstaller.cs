using Data;
using Zenject;

namespace Installer
{
    public class MenuInstaller : MonoInstaller<MenuInstaller>
    {
        public override void InstallBindings()
        {
            // Bindings
            
            Container.Bind<IDataManager>().To<DataManager>().FromNew().AsSingle().NonLazy();
        }
    }
}