using Godot;
using System.Diagnostics;

// ReSharper disable PossibleLossOfFraction

public partial class FogNode : Sprite2D
{
	private const string LightTexturePath = "res://lightsmall.png";
	private const int GridSize = 16;

	private Texture2D LightTexture;
	private Sprite2D Fog;

	private int DisplayWidth => 320 * 2;
	private int DisplayHeight => 180 * 2;

	private Image FogImage = new Image();
	private ImageTexture FogTexture = new ImageTexture();
	private Image LightImage;
	private Vector2I LightOffset;

	public override void _Ready()
	{
		LightTexture = (Texture2D)GD.Load(LightTexturePath);
		LightImage = LightTexture.GetImage();
		LightOffset = new Vector2I(LightTexture.GetWidth() / 2, LightTexture.GetHeight() / 2);

		Fog = GetNode<Sprite2D>("Fog");
		Debug.Assert(Fog != null, "Fog is null!");

		int fogImageWidth = DisplayWidth / GridSize;
		int fogImageHeight = DisplayHeight / GridSize;
		FogImage = Image.CreateEmpty(fogImageWidth, fogImageHeight, false, Image.Format.Rgba8);
		FogImage.Fill(Colors.Black);
		LightImage.Convert(Image.Format.Rgba8);
		Fog.Scale *= GridSize;
	}

	private void UpdateFog(Vector2I newGridPosition)
	{
		Rect2I lightRect = new Rect2I(Vector2I.Zero, new Vector2I(LightImage.GetWidth(), LightImage.GetHeight()));
		FogImage.BlendRect(LightImage, lightRect, newGridPosition - LightOffset);

		UpdateFogImageTexture();
	}

	private void UpdateFogImageTexture()
	{
		// GD.Print($"{FogImage.GetWidth()} {FogImage.GetHeight()}");
		FogTexture = ImageTexture.CreateFromImage(FogImage);
		Fog.Texture = FogTexture;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseEvent)
		{
			Vector2I mousePosition = new Vector2I((int)mouseEvent.Position.X, (int)mouseEvent.Position.Y);
			UpdateFog(mousePosition / GridSize);
		}
	}
}
