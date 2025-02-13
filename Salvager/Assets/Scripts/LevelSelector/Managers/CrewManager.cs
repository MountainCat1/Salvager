using System;
using System.Collections.Generic;
using System.Linq;
using Constants;
using Data;
using Items;
using Managers.LevelSelector;
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
        InGameResources Resources { get; }
        public Guid CurrentLocationId { get; }
        void ChangeCurrentLocation(LocationData toLocation);
        bool CanTravel();
    }

    public class CrewManager : MonoBehaviour, ICrewManager
    {
        public event Action Changed;
        public event Action<CreatureData> SelectedCreature;

        [Inject] private IDataManager _dataManager;
        [Inject] private IRegionManager _regionManager;
        [Inject] private DiContainer _diContainer;

        public ICollection<CreatureData> Crew { get; private set; }
        public InventoryData Inventory { get; private set; }
        public InGameResources Resources { get; private set; }
        public Guid CurrentLocationId { private set; get; }
        public LocationData CurrentLocation { get; private set; }

        [SerializeField] private List<ItemBehaviour> startingItems;
        [SerializeField] private float startingMoney = 50;
        [SerializeField] private float startingFuel = 5;
        [SerializeField] private float startingJuice = 200;
        

        public void SetCrew(GameData gameData)
        {
            Crew = gameData.Creatures;
            Inventory = gameData.Inventory;
            Resources = gameData.Resources;
            CurrentLocationId = Guid.Parse(gameData.CurrentLocationId);
            
            CurrentLocation = _regionManager.Region.Locations.First(l => l.Id == CurrentLocationId);
            
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

            var gameData = _dataManager.GetData();
            
            gameData.Creatures = crew;
            gameData.Inventory = startingInventory;
            gameData.Resources = new InGameResources()
            {
                Money = (decimal)startingMoney,
                Fuel = (decimal)startingFuel,
                Juice = (decimal)startingJuice,
            };
            gameData.CurrentLocationId = _regionManager.Region.Locations.First(l => l.Type == LevelType.StartNode).Id.ToString();

            _dataManager.SaveData();

            Crew = crew;
            Inventory = startingInventory;
            Resources = new InGameResources()
            {
                Money = (decimal)startingMoney,
                Fuel = (decimal)startingFuel,
                Juice = (decimal)startingJuice,
            };

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
        
        public void ChangeCurrentLocation(LocationData selectedLocation)
        {
            if(Resources.Fuel < 1)
            {
                Debug.LogError("Tried to travel while not enough fuel to travel");
                return;
            }
            
            Resources.Fuel -= 1;
            
            CurrentLocation = selectedLocation;
            CurrentLocationId = selectedLocation.Id;

            Changed?.Invoke();
        }

        public bool CanTravel()
        {
            return Resources.Fuel > 0;
        }

        private CreatureData GenerateCrew()
        {
            return new CreatureData()
            {
                Name = $"{Names.Human.RandomElement()} {Surnames.Human.RandomElement()}",
                SightRange = 5f,
                Inventory = new InventoryData()
                {
                },
                CreatureID = Guid.NewGuid().ToString(),
            };
        }
    }
}