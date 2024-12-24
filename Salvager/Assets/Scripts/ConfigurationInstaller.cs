using ScriptableObjects;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ConfigurationInstaller", menuName = "Installers/ConfigurationInstaller")]
public class ConfigurationInstaller : ScriptableObjectInstaller<ConfigurationInstaller>
{
    [SerializeField] private GameConfiguration gameConfiguration;

    public override void InstallBindings()
    {
        Container.Bind<IGameConfiguration>().To<GameConfiguration>().FromInstance(gameConfiguration);
    }
}