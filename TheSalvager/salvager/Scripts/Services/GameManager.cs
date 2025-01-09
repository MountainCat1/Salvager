using System.Diagnostics;
using Godot;
using Services.Abstractions;

namespace Services;

public partial class GameManager : Node2D
{
    [Export] private PackedScene _playerUnitPrefab = null!;
    [Export] private PackedScene _enemyUnitPrefab = null!;
    
    
    [Inject] private ISpawnerManager _spawnerManager = null!;
    [Inject] private IMapGenerator _mapGenerator = null!;
    
    private void Start()
    {
        var map = _mapGenerator.MapData;
        Debug.Assert(map != null, "Map is null!");
        
        _spawnerManager.SpawnCreature(_playerUnitPrefab, map.GetRandomPositionTileOfType(TileType.Floor));
        _spawnerManager.SpawnCreature(_playerUnitPrefab, map.GetRandomPositionTileOfType(TileType.Floor));
        // _spawnerManager.SpawnCreature(_enemyUnitPrefab, map.GetRandomPositionTileOfType(TileType.Floor));
        // _spawnerManager.SpawnCreature(_enemyUnitPrefab, map.GetRandomPositionTileOfType(TileType.Floor));
        // _spawnerManager.SpawnCreature(_enemyUnitPrefab, map.GetRandomPositionTileOfType(TileType.Floor));
        // _spawnerManager.SpawnCreature(_enemyUnitPrefab, map.GetRandomPositionTileOfType(TileType.Floor));
        // _spawnerManager.SpawnCreature(_enemyUnitPrefab, map.GetRandomPositionTileOfType(TileType.Floor));
        // _spawnerManager.SpawnCreature(_enemyUnitPrefab, map.GetRandomPositionTileOfType(TileType.Floor));
    }
}