using Godot;
using System;
using System.Diagnostics;

public partial class ChoiceInstance : Node2D
{
	[Export] public ChoiceResource choiceResource;
	public Choice choice;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Debug_Load_Choice();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void Debug_Load_Choice()
	{
		GD.Print("ChoiceInstance.Debug_Load_Choice");        
		Choice choice = new Choice(choiceResource);
		this.choice = choice;
		this.choice.PrintChoice();
	}
}
