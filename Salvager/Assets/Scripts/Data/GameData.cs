using System;
using System.Collections.Generic;
using System.Linq;
using Items;
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
    [Serializable]
    public class ConnectionData
    {
        public Vector2 From;
        public List<Vector2> To;
        
        public static ConnectionData FromConnection(Vector2 from, List<Vector2> to)
        {
            return new ConnectionData
            {
                From = from,
                To = to
            };
        }
    }
    
    public string Name;
    [FormerlySerializedAs("Levels")] public List<LocationData> Locations;
    public List<ConnectionData> Connections;

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
        };
    }


    public static Location ToLocation(LocationData locationData)
    {
        var settings = GenerateMapSettingsData.ToSettings(locationData.MapSettings);
        
        var level = new Location(settings, locationData.Blueprints, locationData.Name, Guid.Parse(locationData.Id));
        
        level.Type = locationData.Type;
        level.Position = locationData.Position;

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
    public CreatureState State;
    public int XpAmount;
    public float SightRange;
    public Teams Team;
    public float InteractionRange;
    public InventoryData Inventory;

    public static CreatureData FromCreature(Creature creature)
    {
        return new CreatureData
        {
            CreatureID = creature.GetInstanceID().ToString(),
            Name = creature.name,
            State = creature.State,
            XpAmount = creature.XpAmount,
            SightRange = creature.SightRange,
            Team = creature.Team,
            InteractionRange = creature.InteractionRange,
            Inventory = InventoryData.FromInventory(creature.Inventory)
        };
    }
}

[Serializable]
public class InventoryData
{
    public List<ItemData> Items;

    public static InventoryData FromInventory(Inventory inventory)
    {
        var data = new InventoryData { Items = new List<ItemData>() };
        foreach (var item in inventory.Items)
        {
            data.Items.Add(new ItemData
            {
                Identifier = item.GetIdentifier(),
                Count = item.Count,
                Icon = item.Icon,
                Name = item.Name
            });
        }

        return data;
    }

    public void AddItem(ItemData itemData)
    {
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
    public Sprite Icon;

    public static ItemData FromItem(ItemBehaviour item)
    {
        return new ItemData
        {
            Identifier = item.GetIdentifier(),
            Count = item.Count,
            Icon = item.Icon,
            Name = item.Name
        };
    }
}

