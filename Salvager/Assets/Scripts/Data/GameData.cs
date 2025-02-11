using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using LevelSelector.Managers;
using Managers.LevelSelector;
using Services.MapGenerators;
using UnityEngine;
using UnityEngine.Serialization;

// ReSharper disable InconsistentNaming

[Serializable]
public class GameData
{
    public List<CreatureData> Creatures;
    public RegionData Region;
    public string CurrentLocationId;
    public InventoryData Inventory;
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
            Locations = region.Locations.Select(x =>
            {
                var data = LocationData.FromLocation(x);
                return data;
            }).ToList(),
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
            var level = LocationData.ToLocation(levelData);
            region.AddLocation(level);
        }

        // Connect levels
        foreach (var level in region.Locations)
        {
            var levelData = dataRegion.Locations.First(x => x.Id == level.Id.ToString());
            level.Neighbours = levelData.Neighbours.Select(x => region.Locations.First(l => l.Id.ToString() == x)).ToList();
        }

        return region;
    }
}

[System.Serializable]
public class LocationFeatureData
{
    [System.Serializable] public class FeatureEnemyData
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
    public string[] RoomBlueprints;

    public static LocationFeatureData FromLocationFeature(LocationFeature feature)
    {
        return new LocationFeatureData
        {
            Name = feature.name,
            Description = feature.description,
            Enemies = feature.enemies.Select(FeatureEnemyData.FromFeatureEnemy).ToList()
        };
    }
}

[Serializable]
public class LocationData
{
    public string Id;
    public string Name;
    public Vector2 Position;
    public GenerateMapSettingsData MapSettings;
    public RoomBlueprint[] Blueprints;
    public LevelType Type;
    public string[] Neighbours;
    public LocationFeatureData[] Features;
    
    public static LocationData FromLocation(Location location)
    {
        return new LocationData
        {
            Id = location.Id.ToString(),
            Name = location.Name,
            MapSettings = GenerateMapSettingsData.FromSettings(location.Settings),
            Blueprints = location.RoomBlueprints,
            Type = location.Type,
            Position = location.Position,
            Neighbours = location.Neighbours.Select(x => x.Id.ToString()).ToArray(),
            Features = location.Features.ToArray()
        };
    }


    public static Location ToLocation(LocationData locationData)
    {
        var settings = GenerateMapSettingsData.ToSettings(locationData.MapSettings);
        
        var level = new Location(settings, locationData.Blueprints, locationData.Name, Guid.Parse(locationData.Id));
        
        level.Type = locationData.Type;
        level.Position = locationData.Position;
        level.Features = locationData.Features.ToList();

        return level;
    }
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

    public static GenerateMapSettingsData FromSettings(GenerateMapSettings settings)
    {
        return new GenerateMapSettingsData
        {
            roomCount = settings.roomCount,
            roomMinSize = settings.roomMinSize,
            roomMaxSize = settings.roomMaxSize,
            gridSize = settings.gridSize,
            tileSize = settings.tileSize,
            seed = settings.seed
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
            seed = levelDataMapSettings.seed
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

    public static CreatureData FromCreature(Creature creature)
    {
        Inventory inventory = creature.Inventory;
        if(inventory is null)
            inventory = new Inventory(creature.transform.Find("Inventory")); // TODO: make this a const "Inventory"
        
        return new CreatureData
        {
            CreatureID = creature.GetIdentifier(),
            Name = creature.name,
            SightRange = creature.SightRange,
            Inventory = InventoryData.FromInventory(inventory)
        };
    }
}

[Serializable]
public class InventoryData
{
    public List<ItemData> Items = new();

    public static InventoryData FromInventory(Inventory inventory)
    {
        var data = new InventoryData { Items = new List<ItemData>() };
        foreach (var item in inventory.Items)
        {
            data.Items.Add(ItemData.FromItem(item));
        }

        return data;
    }

    public void AddItem(ItemData itemData)
    {
        if(itemData.Stackable == false)
        {
            Items.Add(itemData);
            return;
        }
        
        var item = Items.FirstOrDefault(x => x.Identifier == itemData.Identifier);
        if (item == null)
        {
            Items.Add(itemData);
        }
        else
        {
            item.Count += itemData.Count;
        }
    }
    
    public void RemoveItem(ItemData itemData)
    {
        Items.Remove(itemData);
    }
}

[Serializable]
public class ItemData
{
    public string Identifier;
    public int Count = 1;
    public string Name;
    public string Icon;
    public bool Stackable;

    public static ItemData FromItem(ItemBehaviour item)
    {
        return new ItemData
        {
            Identifier = item.GetIdentifier(),
            Count = item.Count,
            Icon = item.Icon.name,
            Name = item.Name,
            Stackable = item.Stackable
        };
    }
}

