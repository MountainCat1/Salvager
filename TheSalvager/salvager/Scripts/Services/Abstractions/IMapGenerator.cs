using System;
using Services.MapGenerators;

namespace Services.Abstractions;

public interface IMapGenerator
{
    public event Action MapGenerated;
    public MapData? MapData { get; }
}

public enum TileType
{
    Empty = 0,
    Floor = 1,
    Wall = 2,
}