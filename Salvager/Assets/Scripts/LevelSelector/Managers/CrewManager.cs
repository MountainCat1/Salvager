using System;
using System.Collections.Generic;
using System.Linq;
using Constants;
using Data;
using Items;
using UnityEngine;
using Utilities;
using Zenject;

namespace Managers
{
    public interface ICrewManager
    {
        event Action Changed;
        event Action<CreatureData> SelectedCreature;
        void ReRollCrew();
        public ICollection<CreatureData> Crew { get; }
        InventoryData Inventory { get; }
         void SetCrew(GameData gameData);
        void SelectCreature(CreatureData creature);
    }

    public class CrewManager : MonoBehaviour, ICrewManager
    {
        public event Action Changed;
        public event Action<CreatureData> SelectedCreature;

        [Inject] private IDataManager _dataManager;
        [Inject] private DiContainer _diContainer;
        
        public ICollection<CreatureData> Crew { get; private set; }
        public InventoryData Inventory { get; private set; }

        [SerializeField] private List<ItemBehaviour> startingItems;
        
        public void SetCrew(GameData gameData)
        {
            Crew = gameData.Creatures;
            Inventory = gameData.Inventory;
            Changed?.Invoke();
        }

        public void ReRollCrew()
        {
            const int crewCount = 5;
            var crew = new List<CreatureData>();
            for (int i = 0; i < crewCount; i++)
            {
                crew.Add(GenerateCrew());
            }
            
            InventoryData startingInventory = new InventoryData()
            {
                Items = startingItems.Select(ItemData.FromItem).ToList(),
            };
            
            _dataManager.SaveData(new GameData()
            {
                Creatures = crew,
                Inventory = startingInventory
            });
            
            Crew = crew;
            Inventory = startingInventory;
            
            Changed?.Invoke();
        }
        

        public void SelectCreature(CreatureData creature)
        {
            if (!Crew.Contains(creature))
            {
                Debug.LogError("Creature not in crew");
                return;
            }
            
            SelectedCreature?.Invoke(creature);
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
                },
                Team = Teams.Player,
                State = CreatureState.Idle,
                CreatureID = Guid.NewGuid().ToString(),
                XpAmount = 0
            };
        }
    }
}