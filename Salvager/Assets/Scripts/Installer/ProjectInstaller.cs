using Zenject;

namespace Installer
{
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IInputManager>().To<InputManager>().FromComponentsInHierarchy().AsSingle();
        }
    }
}