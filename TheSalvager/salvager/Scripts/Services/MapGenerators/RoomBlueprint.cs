using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using Utilities;

namespace Services.MapGenerators;

public enum PropPosition
{
    Anywhere,
    Center,
    Corner,
    Edge
}

[System.Serializable]
public class RoomBlueprint
{
    public string Name { get; set; } = null!;
    public List<RoomBlueprintProp> Props { get; set; } = new();

    public static RoomBlueprint FromJson(string json)
    {
        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter(),
                new Vector2JsonConverter()
            }
        };

        return JsonSerializer.Deserialize<RoomBlueprint>(json, options)
               ?? throw new System.Exception("Failed to deserialize RoomBlueprint");
    }
}

[System.Serializable]
public class RoomBlueprintProp
{
    public string ScenePath { get; set; } = null!;
    public PropPosition Position { get; set; } = PropPosition.Anywhere;
    public Vector2 Offset { get; set; } = Vector2.Zero;
    public int Count { get; set; } = 1;
}