using System.Diagnostics;
using System.Linq;
using Godot;
using Services.Abstractions;
using Utilities;

namespace Services;

public partial class GameManager : Node2D
{
    [Export] private PackedScene _playerUnitPrefab = null!;
    [Export] private PackedScene _enemyUnitPrefab = null!;


    [Inject] private ISpawnerManager _spawnerManager = null!;
    [Inject] private IMapGenerator _mapGenerator = null!;
    [Inject] private CameraController _cameraController = null!;

    private void Start()
    {
        Engine.MaxFps = 120;

        var map = _mapGenerator.MapData;
        Debug.Assert(map != null, "Map is null!");

        var startingRoom = map.GetAllRooms().First(x => x.IsEntrance);

        var firstUnit = _spawnerManager.SpawnCreature(_playerUnitPrefab,
            (Vector2)startingRoom.Positions.GetRandomElement() * map.TileSize);
        _spawnerManager.SpawnCreature(_playerUnitPrefab,
            (Vector2)startingRoom.Positions.GetRandomElement() * map.TileSize);
        _spawnerManager.SpawnCreature(_playerUnitPrefab,
            (Vector2)startingRoom.Positions.GetRandomElement() * map.TileSize);
        _cameraController.GlobalPosition = firstUnit.GlobalPosition;


        _spawnerManager.SpawnCreature(_enemyUnitPrefab, map.GetRandomPositionTileOfType(TileType.Floor));
        _spawnerManager.SpawnCreature(_enemyUnitPrefab, map.GetRandomPositionTileOfType(TileType.Floor));
        _spawnerManager.SpawnCreature(_enemyUnitPrefab, map.GetRandomPositionTileOfType(TileType.Floor));
        _spawnerManager.SpawnCreature(_enemyUnitPrefab, map.GetRandomPositionTileOfType(TileType.Floor));
        _spawnerManager.SpawnCreature(_enemyUnitPrefab, map.GetRandomPositionTileOfType(TileType.Floor));
        
        NodeUtilities.FindRequiredNodeOfType<CanvasModulate>(this).Color = Colors.Black;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("change_fps"))
        {
            Engine.MaxFps = Engine.MaxFps == 0 ? 120 : 0;
        }

        if (Input.IsActionJustPressed("cheat_vision"))
        {
            var canvasModulate = NodeUtilities.FindRequiredNodeOfType<CanvasModulate>(this);
            if (canvasModulate.Color == Colors.White)
                canvasModulate.Color = Colors.Black;
            else
                canvasModulate.Color = Colors.White;
        }
    }
}