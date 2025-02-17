using Godot;
using System;
using System.Diagnostics;

public partial class CardInstance : Node2D
{
	[Export] public CardResource cardResource;
	public Card card;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Debug_Load_Card();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void Debug_Load_Card()
    {
        GD.Print("GameManager debug");        
        CardResource cardResource = ResourceLoader.Load<CardResource>("res://resources/cards/test_card.tres");
        Card card = new Card(cardResource);
        card.PrintCard();
		this.card = card;
    }
}
