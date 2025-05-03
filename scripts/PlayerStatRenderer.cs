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
	}
}

