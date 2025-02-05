using System;
using System.Collections.Generic;
using System.Linq;
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
    public List<LevelData> Levels;
    public List<ConnectionData> Connections;

    public static RegionData FromRegion(Region region)
    {
        return new RegionData
        {
            Name = region.Name,
            Levels = region.Levels.Select(x =>
            {
                var data = LevelData.FromLevel(x.Value);
                data.Position = x.Key;
                return data;
            }).ToList(),
            Connections = region.Connections.Select(x => ConnectionData.FromConnection(x.Key, x.Value)).ToList()
        };
    }

    public static Region ToRegion(RegionData dataRegion)
    {
        var region = new Region()
        {
            Name = dataRegion.Name
        };

        foreach (var levelData in dataRegion.Levels)
        {
            var level = LevelData.ToLevel(levelData);
            region.AddLevel(level, levelData.Position);
        }
        
        foreach (var connectionData in dataRegion.Connections)
        {
            region.AddConnection(connectionData.From, connectionData.To);
        }

        return region;
    }
}

[Serializable]
public class LevelData
{
    public string Name;
    public Vector2 Position;
    public GenerateMapSettingsData MapSettings;
    public RoomBlueprint[] Blueprints;
    public LevelType Type;
    
    public static LevelData FromLevel(Level level)
    {
        return new LevelData
        {
            Name = level.Name,
            MapSettings = GenerateMapSettingsData.FromSettings(level.Settings),
            Blueprints = level.RoomBlueprints,
            Type = level.Type
        };
    }


    public static Level ToLevel(LevelData levelData)
    {
        var settings = GenerateMapSettingsData.ToSettings(levelData.MapSettings);
        var level = new Level(settings, levelData.Blueprints, levelData.Name);
        level.Type = levelData.Type;

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

    public static GenerateMapSettingsData FromSettings(GenerateMapSettings settings)
    {
        return new GenerateMapSettingsData
        {
            roomCount = settings.roomCount,
            roomMinSize = settings.roomMinSize,
            roomMaxSize = settings.roomMaxSize,
            gridSize = settings.gridSize,
            tileSize = settings.tileSize
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
            tileSize = levelDataMapSettings.tileSize
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
            });
        }

        return data;
    }
}

[Serializable]
public class ItemData
{
    public string Identifier;
    public int Count;
}