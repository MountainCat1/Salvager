using System;
using Services.MapGenerators;
using UnityEngine;

public enum LevelType
{
    Default,
    EndNode,
    StartNode
}

[Serializable]
public class Level
{
    // Accessors
    public GenerateMapSettings Settings => settings;
    public RoomBlueprint[] RoomBlueprints => roomBlueprints;
    public string Name => name;
    public LevelType Type { get; set; } = LevelType.Default;

    // Serialized fields
    [SerializeField] private GenerateMapSettings settings;
    [SerializeField] private RoomBlueprint[] roomBlueprints;
    [SerializeField] private string name;


    public Level()
    {
    }
    
    public Level(GenerateMapSettings settings, RoomBlueprint[] roomBlueprints, string name)
    {
        this.settings = settings;
        this.roomBlueprints = roomBlueprints;
        this.name = name;
    }
    
    public static Level GenerateRandom(RoomBlueprint[] roomBlueprints)
    {
        var settings = new GenerateMapSettings
        {
            roomCount = UnityEngine.Random.Range(10, 20),
            roomMaxSize = new Vector2Int(UnityEngine.Random.Range(5, 10), UnityEngine.Random.Range(5, 10)),
            roomMinSize = new Vector2Int(UnityEngine.Random.Range(3, 4), UnityEngine.Random.Range(3, 4)),
            gridSize = new Vector2Int(UnityEngine.Random.Range(50, 100), UnityEngine.Random.Range(50, 100))
        };

        var spaceStationNames = new string[]
        {
            "Alpha Station",
            "Ice Breaker",
            "The Ark",
            "The Forge",
            "The Nexus",
            "The Outpost",
            "The Spire",
            "The Vault",
            "The Watchtower",
            "The Workshop",
            "Tranquility",
            "Unity Station",
            "Vanguard",
            "Voyager",
            "Wayfarer",
            "Zenith"
        };

        return new Level
        {
            settings = settings,
            roomBlueprints = roomBlueprints,
            name = spaceStationNames[UnityEngine.Random.Range(0, spaceStationNames.Length)]
        };
    }
}