using System;
using Services.MapGenerators;

public interface IMapGenerator
{
    public event Action MapGenerated;
    public MapData MapData { get; }
    
    public void GenerateMap();
}

public enum TileType
{
    Empty = 0,
    Floor = 1,
    Wall = 2,
}