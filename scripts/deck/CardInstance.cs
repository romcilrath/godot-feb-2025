using Godot;
using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class CardInstance : Node2D
{
	[Export] public CardResource cardResource = null;
	private Card _card = null;
	private bool _isFlipped = false;

	[Signal]
    public delegate void OnCardSelectedEventHandler(CardInstance selectedCardInstance);
	[Signal]
    public delegate void OnCardDismissedEventHandler(CardInstance cardInstance);
	[Signal]
	public delegate void OnShakeEventHandler(float degrees = 0.4f, float duration = 0.5f);

	public ChoiceInstance[] choiceInstances { get; private set; }

	[Export] public NodePath CardBack_ { get; set; }
	[Export] public NodePath BackdropPath { get; set; }
	[Export] public NodePath OtherElementsPath { get; set; }
	[Export] public NodePath BodyChoicesContainer { get; set; }
	[Export] public NodePath ArtPath { get; set; }
	[Export] public NodePath WindowPath { get; set; }
	[Export] public NodePath TabPath { get; set; }
	[Export] public NodePath NameLabelPath { get; set; }
	[Export] public NodePath NumberPath { get; set; }
	[Export] public NodePath BodyPath { get; set; }
	[Export] public NodePath ChoicesContainerPath { get; set; }
	[Export] public NodePath TextboxPath { get; set; }
	[Export] public NodePath DivBarPath { get; set; }

	private CardBack _cardBack;
	private NinePatchRect _backdrop;
	private Node2D _otherElements;
	private BoxContainer _bodyChoicesContainer;
	private TextureRect _art;
	private NinePatchRect _window;
	private NinePatchRect _tab;
	private RichTextLabel _nameLabel;
	private RichTextLabel _number;
	private RichTextLabel _body;
	private VBoxContainer _choicesContainer;
	private NinePatchRect _textbox;
	private ReferenceRect _divBar;

	private bool _isEnabled = true;
	
	// Get the nodes from the scene and load the card on Ready
	public override void _Ready()
	{
		_cardBack = NodeUtils.FindNodeWithError<CardBack>(this, CardBack_, "CardBack");
		_backdrop = NodeUtils.FindNodeWithError<NinePatchRect>(this, BackdropPath, "Backdrop");
		_otherElements = NodeUtils.FindNodeWithError<Node2D>(this, OtherElementsPath, "OtherElements");
		_bodyChoicesContainer = NodeUtils.FindNodeWithError<BoxContainer>(this, BodyChoicesContainer, "BodyChoicesContainer");
		_art = NodeUtils.FindNodeWithError<TextureRect>(this, ArtPath, "Art");
		_window = NodeUtils.FindNodeWithError<NinePatchRect>(this, WindowPath, "Window");
		_tab = NodeUtils.FindNodeWithError<NinePatchRect>(this, TabPath, "Tab");
		_nameLabel = NodeUtils.FindNodeWithError<RichTextLabel>(this, NameLabelPath, "NameLabel");
		_number = NodeUtils.FindNodeWithError<RichTextLabel>(this, NumberPath, "Number");
		_body = NodeUtils.FindNodeWithError<RichTextLabel>(this, BodyPath, "Body");
		_choicesContainer = NodeUtils.FindNodeWithError<VBoxContainer>(this, ChoicesContainerPath, "ChoicesContainer");
		_textbox = NodeUtils.FindNodeWithError<NinePatchRect>(this, TextboxPath, "Textbox");
		_divBar = NodeUtils.FindNodeWithError<ReferenceRect>(this, DivBarPath, "DivBar");

		// If cardResource is defined via editor (like for debug) then SetCard and LoadCard
		if (this.cardResource is not null) 
		{
			SetCard(this.cardResource);
			LoadCard();
		}
	}

	public void SetCard(CardResource cardResource)
	{
		// Instiate the card object from the cardResource
		SetCard(new Card(cardResource));
	}

	public void SetCard(Card card)
	{
		// Directly assign the card
		this._card = card;
	}

	public void SetEnabled(bool isEnabled = true)
	{
		this._isEnabled = isEnabled;
		this._cardBack.SetEnabled(isEnabled);
	}

	public Card GetCard()
	{
		return this._card;
	}

	// Load the card visuals including choices
	public void LoadCard() 
	{	
		GD.Print("Loading card " + this._card.Name + "...");

		// Set the card visuals by setting card flipped to true, (state when showing the back)
		// and then calling FlipCard() to show the front and ensure its all visible
		_isFlipped = true;
		FlipCard();

		// Set the card visuals (number, name, art, body)
		_number.Text = "No. " + this._card.Number.ToString();
		_nameLabel.Text = this._card.Name;
		_art.Texture = this._card.Art;
		_body.Text = "[center]" + this._card.Body + "[/center]";

		// Load each choice to the card, includes instantiating 
		// Hooks up choice sigals, adds as children, instantiates, and adds spaces between choices 
		choiceInstances = new ChoiceInstance[_card.Choices.Length];
		for (int i = 0; i < this._card.Choices.Length; i++) 
		{
			Choice choice = this._card.Choices[i];
			choiceInstances[i] = LoadChoice(choice);
		}

		if (this._card.Deck is null) return;

		this._textbox.SelfModulate = this._card.Deck.PrimaryColor;
		this._cardBack.SelfModulate = this._card.Deck.SecondaryColor;
		this._backdrop.SelfModulate = this._card.Deck.SecondaryColor;
		this._tab.SelfModulate = this._card.Deck.SecondaryColor;
		this._window.SelfModulate = this._card.Deck.SecondaryColor;
		this._divBar.SelfModulate = this._card.Deck.SecondaryColor;

		foreach (ChoiceInstance choiceInstance in choiceInstances)
		{
			choiceInstance.SetColor(this._card.Deck.ChoicesColor);
		}
	}

	// Load all choices to the card
	// Hooks up choice sigals, adds as children, instantiates, and adds spaces between choices 
	public ChoiceInstance LoadChoice(Choice choice)
	{
		GD.Print("Loading choice " + choice.Text + "...");

		// Instantiate the choice instance object and set its choiceResource
		ChoiceInstance choiceInstance = GlobalReferences.Instance.ChoiceInstanceScene.Instantiate() as ChoiceInstance;
		_choicesContainer.AddChild(choiceInstance);
		choiceInstance.SetChoice(choice);
		choiceInstance.LoadChoice();

		// Connect the signals
		choiceInstance.Connect(ChoiceInstance.SignalName.OnShake, Callable.From((float degrees, float duration) => EmitSignal(nameof(OnShake), degrees, duration)));
		choiceInstance.Connect(ChoiceInstance.SignalName.OnChoiceSelected, Callable.From(() => OnChoiceSelected()));

		// Add space between choicesf
		ReferenceRect space = new ReferenceRect();
		space.CustomMinimumSize = new Vector2(0, 50);
		_choicesContainer.AddChild(space);

		return choiceInstance;
	}

	public void ClearChoices()
	{
		foreach (Node child in this._choicesContainer.GetChildren())
		{
			child.QueueFree();
		}
	}	

	// "Flips" the cards visuals in the sense that it...
	// shows the card back if the card front is visible and vice versa
	// Does not run the flip animation
	public void FlipCard()
	{
		if (!_isEnabled) return;

		if (_isFlipped)
		{
			_isFlipped = false;

			_cardBack.Visible = false;
			_backdrop.Visible = true;

			_otherElements.Visible = true;
			_bodyChoicesContainer.Visible = true;
			_tab.Visible = true;
			_nameLabel.Visible = true;
		}
		else
		{
			_isFlipped = true;

			_cardBack.Visible = true;
			_backdrop.Visible = false;
			
			_otherElements.Visible = false;
			_bodyChoicesContainer.Visible = false;
			_tab.Visible = false;
			_nameLabel.Visible = false;
		}
	}

	public void OnCardBackSelected()
	{
		EmitSignal(SignalName.OnCardSelected, this);
	}

	public void OnChoiceSelected()
	{
		foreach (ChoiceInstance choiceInstance in choiceInstances)
		{
			choiceInstance.SetDisabled(true);
			GD.Print("ChoiceInstance disabled");
		}
		
		EmitSignal(SignalName.OnCardDismissed, this);
	}
}
