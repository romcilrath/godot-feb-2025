using Godot;
using System;
using System.Diagnostics;

public partial class CardInstance : BaseNode
{
	[Export] public CardResource cardResource;
	private Card _card;

    public ChoiceInstance[] choiceInstances { get; private set; }

    [Export] public NodePath BackdropPath { get; set; }
    [Export] public NodePath ArtPath { get; set; }
    [Export] public NodePath WindowPath { get; set; }
    [Export] public NodePath TabPath { get; set; }
    [Export] public NodePath NameLabelPath { get; set; }
    [Export] public NodePath NumberPath { get; set; }
    [Export] public NodePath BodyPath { get; set; }
    [Export] public NodePath ChoicesContainerPath { get; set; }

    private NinePatchRect _backdrop;
    private TextureRect _art;
    private NinePatchRect _window;
    private NinePatchRect _tab;
    private RichTextLabel _nameLabel;
    private RichTextLabel _number;
    private RichTextLabel _body;
    private VBoxContainer _choicesContainer;

	public override void _Ready()
	{
	    _backdrop = GetNodeWithError<NinePatchRect>(BackdropPath, "Backdrop");
	    _art = GetNodeWithError<TextureRect>(ArtPath, "Art");
	    _window = GetNodeWithError<NinePatchRect>(WindowPath, "Window");
	    _tab = GetNodeWithError<NinePatchRect>(TabPath, "Tab");
	    _nameLabel = GetNodeWithError<RichTextLabel>(NameLabelPath, "NameLabel");
	    _number = GetNodeWithError<RichTextLabel>(NumberPath, "Number");
	    _body = GetNodeWithError<RichTextLabel>(BodyPath, "Body");
	    _choicesContainer = GetNodeWithError<VBoxContainer>(ChoicesContainerPath, "ChoicesContainer");

	    LoadCard();
	}

    public void LoadCard()
    {
        Card card = new Card(cardResource);
		this._card = card;
		
		_number.Text = "No. " + this._card.Number.ToString();
        _nameLabel.Text = this._card.Name;
		_art.Texture = this._card.Art;
		_body.Text = "[center]" + this._card.Body + "[/center]";

		foreach (ChoiceResource choiceResource in this.cardResource.Choices)
		{
			GD.Print("Loading choice...");
			GD.Print(choiceResource.Text);
			ChoiceInstance choiceInstance = GlobalReferences.Instance.ChoiceInstanceScene.Instantiate() as ChoiceInstance;
			_choicesContainer.AddChild(choiceInstance);
			choiceInstance.SetChoiceResource(choiceResource);

			ReferenceRect space = new ReferenceRect();
			space.CustomMinimumSize = new Vector2(0, 100);
			_choicesContainer.AddChild(space);
		}
    }

	public void SetCardResource(CardResource newCardResource)
	{
		cardResource = newCardResource;
		LoadCard();
	}
}
