using Godot;
using System;
using System.Diagnostics;

public partial class CardInstance : Node2D
{
	[Export] public CardResource cardResource;
	public Card card;

    public ChoiceInstance[] choiceInstances { get; private set; }

    [Export] public NinePatchRect Backdrop { get; set; }
    [Export] public TextureRect Art { get; set; }
    [Export] public NinePatchRect Window { get; set; }
    [Export] public NinePatchRect Tab { get; set; }
    [Export] public RichTextLabel NameLabel { get; set; }
    [Export] public RichTextLabel Number { get; set; }
    [Export] public RichTextLabel Body { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Debug_Load_Card();
		Number.Text = this.card.Number.ToString();
        NameLabel.Text = this.card.Name;
		Body.Text = "[center]" + this.card.Body + "[/center]";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

    public void Debug_Load_Card()
    {
        GD.Print("CardInstance.Debug_Load_Card()");       
        Card card = new Card(cardResource);
		this.card = card;
        this.card.PrintCard();
    }
}
