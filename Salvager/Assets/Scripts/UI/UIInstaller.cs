using Zenject;

namespace UI
{
    public class UIInstaller : MonoInstaller<UIInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IPopupManagerUI>().To<PopupManagerUI>().FromComponentInHierarchy().AsSingle();
            
            
            Container.Bind<SelectionDisplayEntryUI>().FromComponentInHierarchy().AsSingle();
            Container.Bind<InventoryUI>().FromComponentInHierarchy().AsSingle();
        }
    }
}