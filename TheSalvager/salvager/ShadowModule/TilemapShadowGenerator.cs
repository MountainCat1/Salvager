using System.Linq;
using Godot;
using Services.Abstractions;
using Services.MapGenerators;

namespace ShadowModule;

public partial class TilemapShadowGenerator : Node2D
{
	[Export] private DungeonGenerator _dungeonGenerator = null!;
	[Export] private TileMapLayer _tileMapLayer = null!;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_dungeonGenerator.MapGenerated += OnMapGenerated;
	}

	private void OnMapGenerated()
	{
		return; // TODO: remove this line
		
		var mapData = _dungeonGenerator.MapData!;
		var walls = mapData
			.GetAllTilePositionsOfType(TileType.Wall)
			.Select(x => new Vector2(x.X, x.Y))
			.ToList();

		var clusters = TileCluster.GetConnectedClusters(walls);

		foreach (var cluster in clusters)
		{
			var wrapPolygon = TilePolygon.GetWrappingPolygon(cluster, mapData.TileSize);
			var shadowCaster = new LightOccluder2D();
			AddChild(shadowCaster);

			var polygon = new OccluderPolygon2D();
			polygon.SetPolygon(wrapPolygon.ToArray());
			polygon.CullMode = OccluderPolygon2D.CullModeEnum.Clockwise;
			
			shadowCaster.SetOccluderPolygon(polygon);
		}
	}
}