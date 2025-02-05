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

[Serializable]
public class Level
{
    // Accessors
    public Guid Id { get; set; }
    public GenerateMapSettings Settings => settings;
    public RoomBlueprint[] RoomBlueprints => roomBlueprints;
    public string Name => name;
    public LevelType Type { get; set; } = LevelType.Default;
    public List<Level> Neighbours { get; set; } = new List<Level>();
    public Vector2 Position { get; set; }

    // Serialized fields
    [SerializeField] private GenerateMapSettings settings;
    [SerializeField] private RoomBlueprint[] roomBlueprints;
    [SerializeField] private string name;


    public Level()
    {
        Id = Guid.NewGuid();
    }
    
    public Level(GenerateMapSettings settings, RoomBlueprint[] roomBlueprints, string name, Guid levelDataId)
    {
        this.settings = settings;
        this.roomBlueprints = roomBlueprints;
        this.name = name;
        Id = levelDataId;
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

        return new Level
        {
            settings = settings,
            roomBlueprints = roomBlueprints,
            name = Names.SpaceStations.RandomElement()
        };
    }
}