using ScriptableObjects;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ConfigurationInstaller", menuName = "Installers/ConfigurationInstaller")]
public class GameConfigurationInstaller : ScriptableObjectInstaller<GameConfigurationInstaller>
{
    [SerializeField] private GameConfiguration gameConfiguration;

    public override void InstallBindings()
    {
        Container.Bind<IGameConfiguration>().To<GameConfiguration>().FromInstance(gameConfiguration);
    }
}