using System;
using System.Collections.Generic;
using Constants;
using Services.MapGenerators;
using UnityEngine;
using Utilities;

public enum LevelType
{
    Default,
    EndNode,
    StartNode
}

public class Location
{
    // Accessors
    public Guid Id { get; set; }
    public GenerateMapSettings Settings => _settings;
    public RoomBlueprint[] RoomBlueprints => _roomBlueprints;
    public string Name => _name;
    public LevelType Type { get; set; } = LevelType.Default;
    public List<Location> Neighbours { get; set; } = new List<Location>();
    public Vector2 Position { get; set; }
    public int DistanceToCurrent { get; set; }

    // Serialized fields
    private GenerateMapSettings _settings;
    private RoomBlueprint[] _roomBlueprints;
    private string _name;


    public Location()
    {
        Id = Guid.NewGuid();
    }
    
    public Location(GenerateMapSettings settings, RoomBlueprint[] roomBlueprints, string name, Guid levelDataId)
    {
        _settings = settings;
        _roomBlueprints = roomBlueprints;
        _name = name;
        Id = levelDataId;
    }
    
    public static Location GenerateRandom(RoomBlueprint[] roomBlueprints)
    {
        var settings = new GenerateMapSettings
        {
            roomCount = UnityEngine.Random.Range(10, 20),
            roomMaxSize = new Vector2Int(UnityEngine.Random.Range(5, 10), UnityEngine.Random.Range(5, 10)),
            roomMinSize = new Vector2Int(UnityEngine.Random.Range(3, 4), UnityEngine.Random.Range(3, 4)),
            gridSize = new Vector2Int(UnityEngine.Random.Range(50, 100), UnityEngine.Random.Range(50, 100)),
            seed = UnityEngine.Random.Range(0, 1000000),
            tileSize = 1f
        };

        return new Location
        {
            _settings = settings,
            _roomBlueprints = roomBlueprints,
            _name = Names.SpaceStations.RandomElement(),
        };
    }
}