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
        event Action ChangedLocation;
        void ReRollCrew();
        public ICollection<CreatureData> Crew { get; }
        InventoryData Inventory { get; }

        public void SetCrew(
            ICollection<CreatureData> creature,
            InventoryData inventory,
            InGameResources resources,
            Guid currentLocationId
        );
        InGameResources Resources { get; }
        public Guid CurrentLocationId { get; }
        void ChangeCurrentLocation(LocationData toLocation);
        bool CanTravel();
        void ToggleCreature(CreatureData creature, bool value);
    }

    public class CrewManager : MonoBehaviour, ICrewManager
    {
        public event Action Changed;
        public event Action ChangedLocation;

        [Inject] private IDataManager _dataManager;
        [Inject] private IRegionManager _regionManager;
        [Inject] private DiContainer _diContainer;

        public ICollection<CreatureData> Crew { get; private set; }
        public InventoryData Inventory { get; private set; }
        public InGameResources Resources { get; private set; }
        public Guid CurrentLocationId { private set; get; }
        public LocationData CurrentLocation { get; private set; }

        [SerializeField] private List<ItemBehaviour> startingItems;

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

            ChangedLocation?.Invoke();
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