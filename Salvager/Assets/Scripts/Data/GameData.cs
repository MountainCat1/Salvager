using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using LevelSelector.Managers;
using Managers.LevelSelector;
using Services.MapGenerators;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

// ReSharper disable InconsistentNaming

[Serializable]
public class GameData
{
    public List<CreatureData> Creatures;
    public RegionData Region;
    public string CurrentLocationId;
    public InventoryData Inventory;
    public InGameResources Resources;
}

[Serializable]
public class InGameResources
{
    public decimal Money = 0;
    public decimal Fuel = 0;
    public decimal Juice = 0;
    
    public event Action Changed;
    
    public void AddMoney(decimal amount)
    {
        Money += amount;
        Changed?.Invoke();
    }
    
    public void AddFuel(decimal amount)
    {
        Fuel += amount;
        Changed?.Invoke();
    }
    
    public void AddJuice(decimal amount)
    {
        Juice += amount;
        Changed?.Invoke();
    }
}


[Serializable]
public class RegionData
{
    public string Name;
    [FormerlySerializedAs("Levels")] public List<LocationData> Locations;

    public static RegionData FromRegion(Region region)
    {
        return new RegionData
        {
            Name = region.Name,
            Locations = region.Locations.ToList(),
        };
    }

    public static Region ToRegion(RegionData dataRegion)
    {
        var region = new Region()
        {
            Name = dataRegion.Name
        };

        // Load levels
        foreach (var levelData in dataRegion.Locations)
        {
            region.AddLocation(levelData);
        }

        // Connect levels
        foreach (var level in region.Locations)
        {
            var levelData = dataRegion.Locations.First(x => x.Id == level.Id);
            levelData.Neighbours = levelData.NeighbourIds
                .Select(x => region.Locations.First(l => l.Id.ToString() == x))
                .Select(x => region.Locations.First(l => l.Id == x.Id))
                .ToList();
        }

        return region;
    }
}

[System.Serializable]
public class LocationFeatureData
{
    [System.Serializable]
    public class FeatureEnemyData
    {
        public CreatureData CreatureData;

        public static FeatureEnemyData FromFeatureEnemy(FeatureEnemies data)
        {
            return new FeatureEnemyData
            {
                CreatureData = CreatureData.FromCreature(data.Creature),
            };
        }
    }

    public string Name = string.Empty;
    public string Description = string.Empty;

    public List<FeatureEnemyData> Enemies = new();
    public string[] RoomBlueprints = Array.Empty<string>();
    public string[] GenericRoomBlueprints = Array.Empty<string>();
}

public enum LocationType
{
    Default,
    EndNode,
    StartNode
}

[Serializable]
public class GenerateMapSettingsData
{
    public int roomCount;
    public Vector2Int roomMinSize;
    public Vector2Int roomMaxSize;
    public Vector2Int gridSize;
    public float tileSize;
    public int seed;
    public GenerateMapSettings.CorridorWidth corridorWidth;

    public static GenerateMapSettingsData FromSettings(GenerateMapSettings settings)
    {
        return new GenerateMapSettingsData
        {
            roomCount = settings.roomCount,
            roomMinSize = settings.roomMinSize,
            roomMaxSize = settings.roomMaxSize,
            gridSize = settings.gridSize,
            tileSize = settings.tileSize,
            seed = settings.seed,
            corridorWidth = settings.corridorWidth
        };
    }

    public static GenerateMapSettings ToSettings(GenerateMapSettingsData levelDataMapSettings)
    {
        return new GenerateMapSettings
        {
            roomCount = levelDataMapSettings.roomCount,
            roomMinSize = levelDataMapSettings.roomMinSize,
            roomMaxSize = levelDataMapSettings.roomMaxSize,
            gridSize = levelDataMapSettings.gridSize,
            tileSize = levelDataMapSettings.tileSize,
            seed = levelDataMapSettings.seed,
            corridorWidth = levelDataMapSettings.corridorWidth
        };
    }
}


[Serializable]
public class CreatureData
{
    public string CreatureID;
    public string Name;
    public float SightRange;
    public InventoryData Inventory;
    public int ManaCost = 1;
    public bool Selected = false;

    public static CreatureData FromCreature(Creature creature)
    {
        Inventory inventory = creature.Inventory;
        if (inventory is null)
            inventory = new Inventory(creature.transform.Find("Inventory")); // TODO: make this a const "Inventory"

        return new CreatureData
        {
            CreatureID = creature.GetIdentifier(),
            Name = creature.name,
            SightRange = creature.SightRange,
            Inventory = InventoryData.FromInventory(inventory),
            ManaCost = creature.XpAmount
        };
    }
}

