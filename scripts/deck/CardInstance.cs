using Godot;
using System;
using System.Diagnostics;
using System.Linq;

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
	
    private Tween _rotationTween;

	public override void _Ready()
	{
		_backdrop = NodeUtils.FindNodeWithError<NinePatchRect>(this, BackdropPath, "Backdrop");
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
	}

	public void SetCardResource(CardResource newCardResource)
	{
		cardResource = newCardResource;
		LoadCard();
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
            .TweenProperty(this, "rotation_degrees", 0, duration)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);
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

		Tween positionTween = CreateTween();
		positionTween
			.TweenInterval(0.6f);
		positionTween
			.Chain()
			.TweenProperty(this, "position", new Vector2(0, 25), 0.1f)
			.SetEase(Tween.EaseType.InOut)
			.AsRelative();
		positionTween
			.TweenInterval(0.05f);
		positionTween
			.Chain()
			.TweenProperty(this, "position", new Vector2(0, -75), 0.1f)
			.SetEase(Tween.EaseType.InOut)
			.AsRelative();
		positionTween
			.TweenInterval(0.05f);
		positionTween
			.Chain()
			.TweenProperty(this, "position", new Vector2(0, 4000), 0.5f)
			.SetEase(Tween.EaseType.In)
			.AsRelative();
	}	
}
