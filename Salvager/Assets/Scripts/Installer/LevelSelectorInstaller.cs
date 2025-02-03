using Data;
using Zenject;

namespace Installer
{
    public class LevelSelectorInstaller : MonoInstaller<LevelSelectorInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IDataManager>().To<DataManager>().FromNew().AsSingle().NonLazy();
        }        
    }
}