using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Items;
using Managers.LevelSelector;
using Newtonsoft.Json;
using UnityEngine;
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

        public void SetCrew(
            ICollection<CreatureData> creature,
            InventoryData inventory,
            InGameResources resources,
            Guid currentLocationId
        );
        void SelectCreature(CreatureData creature);
        InGameResources Resources { get; }
        public Guid CurrentLocationId { get; }
        void ChangeCurrentLocation(LocationData toLocation);
        bool CanTravel();
        void ToggleCreature(CreatureData creature, bool value);
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

        public void SetCrew(
            ICollection<CreatureData> creature,
            InventoryData inventory,
            InGameResources resources,
            Guid currentLocationId
        )
        {
            Crew = creature; 
            
            Inventory = inventory;
            Inventory.Changed += () => Changed?.Invoke();

            Resources = resources;
            Resources.Changed += OnResourcesChanged;

            CurrentLocationId = currentLocationId;

            CurrentLocation = _regionManager.Region.Locations.First(l => l.Id == CurrentLocationId);

            Changed?.Invoke();
            
            SelectCreature(Crew.First());
        }

        private void OnResourcesChanged()
        {
            Debug.Log("Resources changed:\n" + JsonConvert.SerializeObject(Resources));

            Changed?.Invoke();
        }

        public void ReRollCrew()
        {
            throw new NotImplementedException();
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
            if (Resources.Fuel < 1)
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

        public void ToggleCreature(CreatureData creature, bool value)
        {
            creature.Selected = value;
            Changed?.Invoke();
        }
    }
}