using Godot;
using System;

// Enum to define available stats
public enum StatType
{
	Money,
	Vitality,
	Grit,
	Rations,
	Turn
}

public partial class PlayerStatRenderer : Label
{

    [Export] public StatType StatToTrack; // Expose the stat type in the editor

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Update the label text based on the selected stat
        switch (StatToTrack)
        {
            case StatType.Money:
                this.Text = "Money: " + PlayerManager.Instance.Money.Current;
                break;
            case StatType.Vitality:
                this.Text = "Vitality: " + PlayerManager.Instance.Vitality.Current;
                break;
            case StatType.Grit:
                this.Text = "Grit: " + PlayerManager.Instance.Grit.Current;
                break;
            case StatType.Rations:
                this.Text = "Rations: " + PlayerManager.Instance.Rations.Current;
                break;
			case StatType.Turn:
				this.Text = "Turn: " + GameManager.Instance.Turn;
				break;
			default:
				this.Text = "???";
				break;
        }
    }
}