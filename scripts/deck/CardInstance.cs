using Godot;
using System;
using System.Diagnostics;

public partial class CardInstance : Node2D
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
		_backdrop = GetNode<NinePatchRect>(BackdropPath);
        if (_backdrop == null)
        {
            GD.PrintErr($"Backdrop node not found at path: {BackdropPath}");
        }

        _art = GetNode<TextureRect>(ArtPath);
        if (_art == null)
        {
            GD.PrintErr($"Art node not found at path: {ArtPath}");
        }

        _window = GetNode<NinePatchRect>(WindowPath);
        if (_window == null)
        {
            GD.PrintErr($"Window node not found at path: {WindowPath}");
        }

        _tab = GetNode<NinePatchRect>(TabPath);
        if (_tab == null)
        {
            GD.PrintErr($"Tab node not found at path: {TabPath}");
        }

        _nameLabel = GetNode<RichTextLabel>(NameLabelPath);
        if (_nameLabel == null)
        {
            GD.PrintErr($"NameLabel node not found at path: {NameLabelPath}");
        }

        _number = GetNode<RichTextLabel>(NumberPath);
        if (_number == null)
        {
            GD.PrintErr($"Number node not found at path: {NumberPath}");
        }

        _body = GetNode<RichTextLabel>(BodyPath);
        if (_body == null)
        {
            GD.PrintErr($"Body node not found at path: {BodyPath}");
        }

        _choicesContainer = GetNode<VBoxContainer>(ChoicesContainerPath);
        if (_choicesContainer == null)
        {
            GD.PrintErr($"ChoicesContainer node not found at path: {ChoicesContainerPath}");
        }

		LoadCard();
	}

    public void LoadCard()
    {
        Card card = new Card(cardResource);
		this._card = card;
		
		_number.Text = this._card.Number.ToString();
        _nameLabel.Text = this._card.Name;
		_body.Text = "[center]" + this._card.Body + "[/center]";

		foreach (ChoiceResource choiceResource in this.cardResource.Choices)
		{
			GD.Print("Loading choice...");
			GD.Print(choiceResource.Text);
			ChoiceInstance choiceInstance = GlobalReferences.Instance.ChoiceInstanceScene.Instantiate() as ChoiceInstance;
			_choicesContainer.AddChild(choiceInstance);
			choiceInstance.SetChoiceResource(choiceResource);

			ReferenceRect space = new ReferenceRect();
			space.CustomMinimumSize = new Vector2(0, 20);
			_choicesContainer.AddChild(space);
		}
    }

	public void SetCardResource(CardResource newCardResource)
	{
		cardResource = newCardResource;
		LoadCard();
	}
}
