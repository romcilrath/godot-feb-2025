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
    public delegate void OnFlipRightEventHandler();
	[Signal]
    public delegate void OnFlipLeftEventHandler();
	[Signal]
    public delegate void OnExitEventHandler();
	[Signal]
    public delegate void OnScaleCardEventHandler(float xScale = 1f, float yScale = 1f, float duration = 1f);
	[Signal]
	public delegate void OnShakeEventHandler(float degrees = 0.4f, float duration = 0.5f);
	[Signal]
	public delegate void OnAnimateCardEffectsEventHandler(float delay = 1f);

	public ChoiceInstance[] choiceInstances { get; private set; }

	[Export] public NodePath CardBack { get; set; }
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
	
    private Tween _rotationTween;

	// Get the nodes from the scene and load the card on Ready
	public override void _Ready()
	{
		_cardBack = NodeUtils.FindNodeWithError<CardBack>(this, CardBack, "CardBack");
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
		foreach (Choice choice in this._card.Choices)
		{
			LoadChoice(choice);
		}
	}

	// Load all choices to the card
	// Hooks up choice sigals, adds as children, instantiates, and adds spaces between choices 
	public void LoadChoice(Choice choice)
	{
		GD.Print("Loading choice " + choice.Text + "...");

		// Instantiate the choice instance object and set its choiceResource
		ChoiceInstance choiceInstance = GlobalReferences.Instance.ChoiceInstanceScene.Instantiate() as ChoiceInstance;
		_choicesContainer.AddChild(choiceInstance);
		choiceInstance.SetChoice(choice);
		choiceInstance.LoadChoice();

		// Connect the signals
		choiceInstance.Connect(ChoiceInstance.SignalName.OnShake, Callable.From((float degrees, float duration) => EmitSignal(nameof(OnShake), degrees, duration)));
		choiceInstance.Connect(ChoiceInstance.SignalName.ChoiceSelected, Callable.From(OnChoiceSelected));
		choiceInstance.Connect(ChoiceInstance.SignalName.ChoiceSelected, Callable.From(OnDismissCard));

		// Add space between choices
		ReferenceRect space = new ReferenceRect();
		space.CustomMinimumSize = new Vector2(0, 50);
		_choicesContainer.AddChild(space);
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
	
	// Disables all choices when a choice is selected
	private void OnChoiceSelected()
	{
		foreach (Node child in _choicesContainer.GetChildren())
		{
			if (child is ChoiceInstance choiceInstance)
			{
				choiceInstance.SetDisabled();
			}
		}
	}
	
	// Dismisses the card when a choice is selected by emitting a signal
	private void OnDismissCard()
	{
		_rotationTween?.Kill();

		EmitSignal(SignalName.OnExit, GameManager.Instance.CardExitPoint.Position.X, GameManager.Instance.CardExitPoint.Position.Y, 1f);
	}	

	private void OnCardBackSelected()
	{
		EmitSignal(SignalName.OnFlipRight);
		EmitSignal(SignalName.OnScaleCard, 0.45f, 0.45f, 1f);
		EmitSignal(SignalName.OnAnimateCardEffects, 2f);
	}
}
