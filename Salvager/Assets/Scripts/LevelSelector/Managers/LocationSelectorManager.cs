using System;
using System.Collections.Generic;
using System.Linq;
using Constants;
using Data;
using Items;
using LevelSelector;
using LevelSelector.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using Zenject;

namespace Managers.LevelSelector
{
    public class LocationSelectorManager : MonoBehaviour
    {
        [Inject] private IRegionGenerator _regionGenerator;
        [Inject] private IRegionManager _regionManager;
        [Inject] private IDataManager _dataManager;
        [Inject] private ICrewManager _crewManager;
        [Inject] private IItemManager _itemManager;

        [SerializeField] private bool skipLoad = false;

        [SerializeField] private int startingCrewSize = 3;
        [SerializeField] private List<ItemBehaviour> startingItems;
        [SerializeField] private float startingMoney = 50;
        [SerializeField] private float startingFuel = 5;
        [SerializeField] private float startingJuice = 200;
        
        [SerializeField] private RegionType firstRegionType;

        [SerializeField] private SceneReference mainMenuScene;

        [Inject] private IInputManager _inputManager;

        private void Start()
        {
            _inputManager.GoBackToMenu += GoBackToMenu;

            var data = _dataManager.LoadData();

            if (skipLoad || data?.Region == null)
            {
                var region = _regionGenerator.Generate(firstRegionType);
                var currentNodeId = region.Locations.First(x => x.Type == LocationType.StartNode).Id;
                _regionManager.SetRegion(region);


                var startingInventory = new InventoryData()
                {
                    Items = startingItems.Select(ItemData.FromPrefabItem).ToList(),
                };
                var startingCreatures = new List<CreatureData>();
                for (int i = 0; i < startingCrewSize; i++)
                {
                    startingCreatures.Add(GenerateCrew());
                }

                var startingResources = new InGameResources()
                {
                    Money = (decimal)startingMoney,
                    Fuel = (decimal)startingFuel,
                    Juice = (decimal)startingJuice,
                };

                _crewManager.SetCrew(startingCreatures, startingInventory, startingResources, currentNodeId);

                var gameData = _dataManager.GetData();

                gameData.Region = _regionManager.Region;
                gameData.Creatures = _crewManager.Crew.ToList();
                gameData.Inventory = _crewManager.Inventory;
                gameData.CurrentLocationId = currentNodeId.ToString();
                gameData.Resources = _crewManager.Resources;

                _dataManager.SaveData();
            }

            LoadData();
        }

        private void GoBackToMenu()
        {
            _dataManager.SaveData();
            SceneManager.LoadScene(mainMenuScene);
        }

        private void LoadData()
        {
            var data = _dataManager.LoadData();

            _regionManager.SetRegion(data.Region);

            _dataManager.DoStuffToDataSoItWorks(data);

            _crewManager.SetCrew(data.Creatures, data.Inventory, data.Resources, Guid.Parse(data.CurrentLocationId));
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