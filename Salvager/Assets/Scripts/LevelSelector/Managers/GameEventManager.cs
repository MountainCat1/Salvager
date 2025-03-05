using System;
using Managers;
using Managers.LevelSelector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace LevelSelector.Managers
{
    public class GameEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public GameEventOption[] Options { get; set; }
    }

    public class GameEventOption
    {
        public string Name { get; set; }
        public Action OnClick { get; set; }
    }
    
    public interface IGameEventManager
    {
        public event Action<GameEvent> EventTriggered;
    }
    
    public class GameEventManager : MonoBehaviour, IGameEventManager
    {
        public event Action<GameEvent> EventTriggered;

        [Inject] private ICrewManager _crewManager;
        [Inject] private IRegionManager _regionManager;

        private void Start()
        {
            _crewManager.ChangedLocation += OnLocationChanged;
            _crewManager.Changed += OnCrewChanged;

            if (_crewManager.Crew.Count == 0)
            {
                var gameEvent = new GameEvent
                {
                    Name = "All dead",
                    Description = "All of your crew members have died. You have failed your mission.",
                    Options = new []
                    {
                        new GameEventOption()
                        {
                            Name = "Emptiness...",
                            OnClick = () =>
                            {
                                SceneManager.LoadScene("Menu");
                            }
                        }
                    }
                };
                
                EventTriggered?.Invoke(gameEvent);       
            }
            
            // var currentLocation = _regionManager.Region.GetLocation(_crewManager.CurrentLocationId);
            //
            // if (currentLocation.Type == LocationType.EndNode && currentLocation.Visited)
            // {
            //     var gameEvent = new GameEvent
            //     {
            //         Name = $"Defeat at {currentLocation.Name}",
            //         Description = $"You have been defeated at {currentLocation.Name}. You have failed your mission.",
            //         Options = new []
            //         {
            //             new GameEventOption()
            //             {
            //                 Name = "Emptiness...",
            //             }
            //         }
            //     };
            //     
            //     EventTriggered?.Invoke(gameEvent);
            // }
        }

        private void OnCrewChanged()
        {
            if(_crewManager.Resources.Fuel <= 0 && _regionManager.Region.GetLocation(_crewManager.CurrentLocationId).ShopData == null)
            {
                var gameEvent = new GameEvent
                {
                    Name = "Out of fuel",
                    Description = "You have run out of fuel. You are stranded in space.",
                    Options = new []
                    {
                        new GameEventOption()
                        {
                            Name = "Emptiness...",
                            OnClick = () =>
                            {
                                SceneManager.LoadScene("Menu");
                            }
                        }
                    }
                };
                
                EventTriggered?.Invoke(gameEvent);
            }
        }

        private void OnLocationChanged()
        {
            var location = _regionManager.Region.GetLocation(_crewManager.CurrentLocationId);
            
            if(location.Type == LocationType.BossNode)
            {
                var gameEvent = new GameEvent
                {
                    Name = "Danger standing in our way",
                    Description = $"We encounter readings of some powerful creature on this station. We should be careful.",
                    Options = new []
                    {
                        new GameEventOption()
                        {
                            Name = "We are ready...",
                        }
                    }
                };
                
                EventTriggered?.Invoke(gameEvent);
            }
        }
    }
}