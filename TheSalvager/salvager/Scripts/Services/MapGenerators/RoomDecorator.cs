using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Utilities;
using FileAccess = Godot.FileAccess;

namespace Services.MapGenerators;

public interface IRoomDecorator
{
    public void DecorateRooms(ICollection<RoomData> roomData, float tileSize);
}

public partial class RoomDecorator : Node2D, IRoomDecorator
{
    [Inject] private IDIContext _context = null!;
    
    private const string DataPath = "res://Data/RoomData/";
    
    public void DecorateRooms(ICollection<RoomData> roomData, float tileSize)
    {
        // Get blueprints from json files inside DataPath
        var roomBlueprints = new List<RoomBlueprint>();
        // implement
        GD.Print("Loading room blueprints");
        
        DirAccess dirAccess = DirAccess.Open(DataPath);
        if (dirAccess == null)
            throw new DirectoryNotFoundException($"Directory {DataPath} not found");
        string[] files = dirAccess.GetFiles();
        
        foreach (var file in files)
        {
            var jsonFile = ResourceLoader.Load<Godot.Json>(DataPath + file);
            GD.Print($"Loading blueprint from {file}");

            
            var blueprint = RoomBlueprint.FromJson(jsonFile.Data.AsString());
            roomBlueprints.Add(blueprint);
        }
        
        // TODO: Implement room decoration blueprint selection
        DecorateRoom(roomData.First(), roomBlueprints.First(), tileSize);
    }
    
    private void DecorateRoom(RoomData roomData, RoomBlueprint blueprint, float tileSize)
    {
        GD.Print($"Decorating room {roomData.RoomID} with blueprint {blueprint.Name}");
        // TODO: Implement room decoration positioning
        foreach (var prop in blueprint.Props)
        {
            var randomPosition = roomData.Positions.GetRandomElement();
            
            var loadProp = ResourceLoader.Load<PackedScene>(prop.ScenePath);
            
            InstantiatePrefab(loadProp, (Vector2)randomPosition * tileSize);
        }
    }
    
    private void InstantiatePrefab(PackedScene prefab, Vector2 position)
    {
        GD.Print($"Instantiating prefab {prefab.ResourceName} at {position}");
        var instance = prefab.Instantiate() as Node2D ?? throw new InvalidCastException("Prefab is not a Node2D");
        _context.FullInject(instance);
        AddChild(instance);
        instance.Position = position;
    }
}