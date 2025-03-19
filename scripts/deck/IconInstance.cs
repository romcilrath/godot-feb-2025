using Godot;
using System;

public partial class IconInstance : TextureRect
{
	private Effect _effect;

    // These are the paths to the child nodes of the IconInstance
    // Allows us to make IconInstance a resource and set the paths in the editor
	[Export] public NodePath CountBoxContainerPath { get; set; }
	[Export] public NodePath CountLabelPath { get; set; }
	[Export] public NodePath ArtPath { get; set; }

    // These are the child nodes of the ChoiceInstance
	private HBoxContainer _countBoxContainer;
	private Label _countLabel;
	private TextureRect _art;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        // Find the child nodes
		_countBoxContainer = NodeUtils.FindNodeWithError<HBoxContainer>(this, CountBoxContainerPath, "CountBoxContainer");
		_countLabel = NodeUtils.FindNodeWithError<Label>(this, CountLabelPath, "CountLabel");
		_art = NodeUtils.FindNodeWithError<TextureRect>(this, ArtPath, "Art");
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
					break;
				case "Money":
					_art.Texture = GlobalReferences.Instance.MoneyIcon;
					break;
				case "Rations":
					_art.Texture = GlobalReferences.Instance.RationsIcon;
					break;
				case "Grit":
					_art.Texture = GlobalReferences.Instance.GritIcon;
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
