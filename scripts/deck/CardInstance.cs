using Godot;
using System;
using System.Diagnostics;
using System.Linq;

public partial class CardInstance : Node2D
{
	[Export] public CardResource cardResource;
	private Card _card;
	private bool _isFlipped = false;
	
    [Signal]
    public delegate void OnFlipEventHandler();

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

	private NinePatchRect _cardBack;
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

	public override void _Ready()
	{
		_cardBack = NodeUtils.FindNodeWithError<NinePatchRect>(this, CardBack, "CardBack");
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

		LoadCard();
	}

	public void LoadCard() 
	{
		GD.Print("Loading card " + cardResource.Name + "...");
		Card card = new Card(cardResource);
		this._card = card;

		_isFlipped = true;
		FlipCard();
		_number.Text = "No. " + this._card.Number.ToString();
		_nameLabel.Text = this._card.Name;
		_art.Texture = this._card.Art;
		_body.Text = "[center]" + this._card.Body + "[/center]";

		foreach (ChoiceResource choiceResource in this.cardResource.Choices)
		{
			LoadChoice(choiceResource);
		}
	}

	public void LoadChoice(ChoiceResource choiceResource)
	{
		GD.Print("Loading choice " + choiceResource.Text + "...");

		ChoiceInstance choiceInstance = GlobalReferences.Instance.ChoiceInstanceScene.Instantiate() as ChoiceInstance;
		_choicesContainer.AddChild(choiceInstance);
		choiceInstance.SetChoiceResource(choiceResource);

		// Connect the ShakeParent signal dynamically using Connect()
		choiceInstance.Connect(ChoiceInstance.SignalName.ShakeParent, Callable.From((float degrees, float duration) => OnShakeParentReceived(degrees, duration)));

		// Connect the Choice signal dynamically using Connect()
		choiceInstance.Connect(ChoiceInstance.SignalName.ChoiceSelected, Callable.From(OnChoiceSelected));
		choiceInstance.Connect(ChoiceInstance.SignalName.ChoiceSelected, Callable.From(OnDismissCard));

		// Optional: Add spacing between choices
		ReferenceRect space = new ReferenceRect();
		space.CustomMinimumSize = new Vector2(0, 50);
		_choicesContainer.AddChild(space);
	}

	public void SetCardResource(CardResource newCardResource)
	{
		cardResource = newCardResource;
		LoadCard();
	}

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

	private void OnShakeParentReceived(float degrees=0.4f, float duration=0.05f)
	{		
		_rotationTween = CreateTween();
		_rotationTween
			.TweenProperty(this, "rotation_degrees", degrees, duration)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Elastic);
		_rotationTween
			.Chain()
			.TweenProperty(this, "rotation_degrees", -degrees, duration)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Elastic);
		_rotationTween
			.Chain()
			.TweenProperty(this, "rotation_degrees", degrees / 2, duration * 1.5f)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Back);
		_rotationTween
			.Chain()
			.TweenProperty(this, "rotation_degrees", -degrees / 2, duration * 1.5f)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Back);
		_rotationTween
			.Chain()
			.TweenProperty(this, "rotation_degrees", 0, duration * 2)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Sine);
	}
	
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
	
	private void OnDismissCard()
	{
		_rotationTween?.Kill();

		EmitSignal(SignalName.OnFlip);
	}	
}
