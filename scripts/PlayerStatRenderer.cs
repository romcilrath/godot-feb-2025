using Godot;
using System;

// Enum to define available stats
public enum StatType
{
	Coin,
	Vitality,
	Grit,
	Rations,
	Turn
}

public partial class PlayerStatRenderer : Node2D
{
	[Export] public Label StatName;
	[Export] public Label StatValue;
    [Export] public TextureRect ArtRect;
	[Export] public ReferenceRect SpaceRect;
	[Export] public StatType StatToTrack;
	[Export] public Texture2D Art;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        // Set the texture of the ArtRect to the specified Art
        if (Art != null)
        {
            ArtRect.Texture = Art;
        }
        else
        {
            GD.PrintErr("Art texture is not set.");
			ArtRect.Visible = false;
			SpaceRect.Visible = false;
        }

        if (StatName == null || StatValue == null)
        {
            GD.PrintErr("StatName or StatValue is not set.");
            return;
        }
        
        if (PlayerManager.Instance == null)
        {
            GD.PrintErr("PlayerManager instance is not available.");
            return;
        }
        
        if (GameManager.Instance == null)
        {
            GD.PrintErr("GameManager instance is not available.");
            return;
        }
        
        GD.Print("PlayerStatRenderer Initialized.");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Update the label text based on the selected stat
		switch (StatToTrack)
		{
			case StatType.Coin:
				StatName.Text = "Coin";
				StatValue.Text = PlayerManager.Instance.Coin.Current.ToString();
				break;
			case StatType.Vitality:
				StatName.Text = "Vitality";
				StatValue.Text = PlayerManager.Instance.Vitality.Current.ToString();
				break;
			case StatType.Grit:
				StatName.Text = "Grit";
				StatValue.Text = PlayerManager.Instance.Grit.Current.ToString();
				break;
			case StatType.Rations:
				StatName.Text = "Rations";
				StatValue.Text = PlayerManager.Instance.Rations.Current.ToString();
				break;
			case StatType.Turn:
				StatName.Text = "Turn";
				StatValue.Text = GameManager.Instance.Turn.ToString();
				break;
			default:
				StatName.Text = "???";
				StatValue.Text = "???";
				break;
		}
		// Adjust pivot offset to center the shake effect
		StatValue.PivotOffset = new Vector2(StatValue.Size.X / 2, StatValue.Size.Y / 2);
	}

	public void Shake(Color? color = null)
	{		
		GD.Print("Shaking " + StatName.Text);
		Tween colorTween = GetTree().CreateTween();
		Tween scaleTween = GetTree().CreateTween();
		Tween rotationTween = GetTree().CreateTween();
		Color originalColor = StatValue.GetThemeColor("font_color");
		Vector2 originalScale = StatValue.Scale;
		float originalRotation = StatValue.Rotation;

		// Use the provided color or fallback to the original color
		Color targetColor = color ?? originalColor;

		colorTween.TweenProperty(StatValue, "theme_override_colors/font_color", targetColor, 0.1f)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.Out);
		colorTween.TweenCallback(Callable.From(() => {}))
			.SetDelay(0.9f);
		colorTween.TweenProperty(StatValue, "theme_override_colors/font_color", originalColor, 0.3f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.InOut);

		// Shake rotation with pivot offset considered
		rotationTween.TweenProperty(StatValue, "rotation", originalRotation + Mathf.DegToRad(30), 0.1f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
		rotationTween.TweenProperty(StatValue, "rotation", originalRotation - Mathf.DegToRad(20), 0.2f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.InOut);
		rotationTween.TweenProperty(StatValue, "rotation", originalRotation + Mathf.DegToRad(20), 0.2f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.InOut);
		rotationTween.TweenProperty(StatValue, "rotation", originalRotation - Mathf.DegToRad(20), 0.2f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.InOut);
		rotationTween.TweenProperty(StatValue, "rotation", originalRotation + Mathf.DegToRad(10), 0.15f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.InOut);
		rotationTween.TweenProperty(StatValue, "rotation", originalRotation, 0.2f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.In);

		// Scale up and back with pivot offset considered
		scaleTween.TweenProperty(StatValue, "scale", originalScale * 1.6f, 0.3f)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.Out);
		scaleTween.TweenProperty(StatValue, "scale", originalScale * 1.3f, 0.1f)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.Out);
		scaleTween.TweenProperty(StatValue, "scale", originalScale * 1.5f, 0.1f)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.Out);
		scaleTween.TweenProperty(StatValue, "scale", originalScale, 0.5f)
			.SetTrans(Tween.TransitionType.Elastic)
			.SetEase(Tween.EaseType.In);
	}
}
