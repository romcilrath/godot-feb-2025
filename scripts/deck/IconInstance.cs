using Godot;
using System;

public partial class IconInstance : TextureRect
{
	private Effect _effect;

    // These are the paths to the child nodes of the IconInstance
    // Allows us to make IconInstance a resource and set the paths in the editor
	[Export] public NodePath CountBoxContainerPath { get; set; }
	[Export] public NodePath CountLabelPath { get; set; }
	[Export] public NodePath ArtContainerPath { get; set; }
	[Export] public NodePath ArtPath { get; set; }

    // These are the child nodes of the ChoiceInstance
	private HBoxContainer _countBoxContainer;
	private Label _countLabel;
	private MarginContainer _artContainer;
	private TextureRect _art;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        // Find the child nodes
		_countBoxContainer = NodeUtils.FindNodeWithError<HBoxContainer>(this, CountBoxContainerPath, "CountBoxContainer");
		_countLabel = NodeUtils.FindNodeWithError<Label>(this, CountLabelPath, "CountLabel");
		_artContainer = NodeUtils.FindNodeWithError<MarginContainer>(this, ArtContainerPath, "ArtContainer");
		_art = NodeUtils.FindNodeWithError<TextureRect>(this, ArtPath, "Art");
	}

	public void SetArtContainerMargins(int margin=40)
	{
		_artContainer.AddThemeConstantOverride("margin_right", margin);
		_artContainer.AddThemeConstantOverride("margin_left", margin);
		_artContainer.AddThemeConstantOverride("margin_top", margin);
		_artContainer.AddThemeConstantOverride("margin_bottom", margin);
	}

	public void SetArtContainerMargins(int marginRight=40, int marginLeft=40, int marginTop=40, int marginBottom=40)
	{
		_artContainer.AddThemeConstantOverride("margin_right", marginRight);
		_artContainer.AddThemeConstantOverride("margin_left", marginLeft);
		_artContainer.AddThemeConstantOverride("margin_top", marginTop);
		_artContainer.AddThemeConstantOverride("margin_bottom", marginBottom);
	}

	public void LoadEffect()
	{
		// Check if the effect is of type StatEffect
		if (this._effect is StatEffect statEffect)
		{
			// Assign apropriate icon to the art node
			switch(statEffect.Target.Name)
			{
				case "Vitality":
					_art.Texture = GlobalReferences.Instance.VitalityIcon;
					this.SelfModulate = GlobalReferences.Instance.VitalityColor;
					SetArtContainerMargins(20);
					break;
				case "Coin":
					_art.Texture = GlobalReferences.Instance.CoinIcon;
					this.SelfModulate = GlobalReferences.Instance.CoinColor;
					SetArtContainerMargins(10, 10, 0, 30);
					break;
				case "Rations":
					_art.Texture = GlobalReferences.Instance.RationsIcon;
					this.SelfModulate = GlobalReferences.Instance.RationsColor;
					SetArtContainerMargins(50);
					break;
				case "Grit":
					_art.Texture = GlobalReferences.Instance.GritIcon;
					this.SelfModulate = GlobalReferences.Instance.GritColor;
					SetArtContainerMargins(0);
					break;
				default:
					// Set the art texture to null
					_art.Texture = null;
					break;
			}
			// Set the count label text to the effect's count
			switch(statEffect.ActionType)
			{
				case ActionType.Add:
					if (statEffect.Value >= 0)
						_countLabel.Text = "+" + statEffect.Value.ToString();
					else
						_countLabel.Text = statEffect.Value.ToString();
					break;
				case ActionType.Set:
					_countLabel.Text = "=" + statEffect.Value.ToString();
					break;
				case ActionType.Multiply:
					_countLabel.Text = "x" + statEffect.Value.ToString();
					break;
				default:
					// Set the count label text to an "???" string
					_countLabel.Text = "???";
					break;
			}
		}
		else
		{
			_countBoxContainer.Visible = false;
		}
	}

	public void SetEffect(Effect newEffect)
	{
        // Set the effect and load the effect
		_effect = newEffect;
		LoadEffect();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
