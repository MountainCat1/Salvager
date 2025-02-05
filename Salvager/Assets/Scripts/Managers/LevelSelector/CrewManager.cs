using System;
using System.Collections.Generic;
using Constants;
using Data;
using UnityEngine;
using Utilities;
using Zenject;

namespace Managers
{
    public interface ICrewManager
    {
        event Action Changed;
        void ReRollCrew();
        public ICollection<CreatureData> Crew { get; }
    }

    public class CrewManager : MonoBehaviour, ICrewManager
    {
        public event Action Changed;
        
        [Inject] private IDataManager _dataManager;
        [Inject] private DiContainer _diContainer;

        public ICollection<CreatureData> Crew { get; private set; }

        public void ReRollCrew()
        {
            const int crewCount = 5;
            var crew = new List<CreatureData>();
            for (int i = 0; i < crewCount; i++)
            {
                crew.Add(GenerateCrew());
            }

            _dataManager.SaveData(new GameData()
            {
                Creatures = crew
            });
            
            Changed?.Invoke();
        }

        private CreatureData GenerateCrew()
        {
            return new CreatureData()
            {
                Name = $"{Names.Human.RandomElement()} {Surnames.Human.RandomElement()}",
                InteractionRange = 1.5f,
                SightRange = 5f,
                Inventory = new InventoryData()
                {
                    Items = new List<ItemData>()
                    {
                        new ItemData()
                        {
                            Identifier = "Gun",
                            Count = 1
                        }
                    }
                },
                Team = Teams.Player,
                State = CreatureState.Idle,
                CreatureID = Guid.NewGuid().ToString(),
                XpAmount = 0
            };
        }
    }
}