public enum LevelType
{
    Default,
    EndNode,
    StartNode
}

// public class Location
// {
//     // Accessors
//     public Guid Id { get; set; }
//     public GenerateMapSettings Settings { get; set; }
//
//     public RoomBlueprint[] RoomBlueprints => _roomBlueprints;
//     public string Name { get; set; }
//
//     public LevelType Type { get; set; } = LevelType.Default;
//     public List<Location> Neighbours { get; set; } = new List<Location>();
//     public Vector2 Position { get; set; }
//     public bool Visited { get; set; } = false;
//     public List<LocationFeatureData> Features { get; set;  } = new();
//
//     // Serialized fields
//     private RoomBlueprint[] _roomBlueprints;
//
//
//     public Location()
//     {
//         Id = Guid.NewGuid();
//     }
//     
//     public Location(GenerateMapSettings settings, RoomBlueprint[] roomBlueprints, string name, Guid levelDataId)
//     {
//         Settings = settings;
//         _roomBlueprints = roomBlueprints;
//         Name = name;
//         Id = levelDataId;
//     }
//     
//     public static Location GenerateRandom(RoomBlueprint[] roomBlueprints)
//     {
//         var settings = GenerateMapSettings.GenerateRandom();
//
//         return new Location
//         {
//             Settings = settings,
//             _roomBlueprints = roomBlueprints,
//             Name = Names.SpaceStations.RandomElement(),
//         };
//     }
// }