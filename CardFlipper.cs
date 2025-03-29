using Godot;
using System;
using System.Diagnostics;

public partial class CardFlipper : Node2D
{
	[Export] public NodePath SubViewportContainer { get; set; }
	[Export] public NodePath CardInstance { get; set; }
	[Export] public NodePath CardEffectsContainer { get; set; }

	private SubViewportContainer _subViewportContainer;
	private CardInstance _cardInstance;
	private VBoxContainer _cardEffectsContainer;
	private IconInstance[] _iconInstances;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_cardInstance = NodeUtils.FindNodeWithError<CardInstance>(this, CardInstance, "CardInstance");
		_subViewportContainer = NodeUtils.FindNodeWithError<SubViewportContainer>(this, SubViewportContainer, "SubViewportContainer");
		_cardEffectsContainer = NodeUtils.FindNodeWithError<VBoxContainer>(this, CardEffectsContainer, "CardEffectsContainer");
		_cardInstance.Connect("OnFlipRight", Callable.From(OnFlipRight));
		_cardInstance.Connect("OnFlipLeft", Callable.From(OnFlipLeft));
		_cardInstance.Connect("OnShake", Callable.From((float degrees, float duration) => OnShake(degrees, duration)));
	}

	public CardInstance GetCardInstance()
	{
		return this._cardInstance;
	}

	public void SetCardInstance(CardInstance cardInstance)
	{
		this._cardInstance = cardInstance;
	}

	public void LoadCardEffects(Effect[] effects)
	{
		_iconInstances = new IconInstance[effects.Length];
		for (int i = 0; i < effects.Length; i ++)
		{
			IconInstance iconInstance = GlobalReferences.Instance.IconInstanceScene.Instantiate() as IconInstance;
			_iconInstances[i] = iconInstance;
			_cardEffectsContainer.AddChild(iconInstance);
			iconInstance.SetEffect(effects[i]);

			ReferenceRect space = new ReferenceRect();
			space.CustomMinimumSize = new Vector2(50, 0);
			_cardEffectsContainer.AddChild(space);
		}
	}

	public void ShowEffects()
	{
		_cardEffectsContainer.Visible = true;
	}

	public void HideEffects()
	{
		_cardEffectsContainer.Visible = false;
	}

	public void DoShake(float degrees=0.4f, float duration=0.05f)
	{
		Tween _rotationTween = CreateTween();
		_rotationTween
			.TweenProperty(_subViewportContainer, "rotation_degrees", degrees, duration)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Elastic);
		_rotationTween
			.Chain()
			.TweenProperty(_subViewportContainer, "rotation_degrees", -degrees, duration)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Elastic);
		_rotationTween
			.Chain()
			.TweenProperty(_subViewportContainer, "rotation_degrees", degrees / 2, duration * 1.5f)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Back);
		_rotationTween
			.Chain()
			.TweenProperty(_subViewportContainer, "rotation_degrees", -degrees / 2, duration * 1.5f)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Back);
		_rotationTween
			.Chain()
			.TweenProperty(_subViewportContainer, "rotation_degrees", 0, duration * 2)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Sine);
	}

	public void DoFlip(bool isFlipRight = true)
	{
		int flipMultiplier = 1;
		if (!isFlipRight) flipMultiplier = -1;

		Tween xRotationTween = CreateTween();
		xRotationTween
			.TweenProperty(_subViewportContainer.Material, "shader_parameter/rotation_x", 15.0f, 0.65f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.In)
			.AsRelative();
		xRotationTween
			.TweenProperty(_subViewportContainer.Material, "shader_parameter/rotation_x", -15.0f, 0.35f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out)
			.AsRelative();

		Tween yRotationTween = CreateTween();
		yRotationTween
			.TweenProperty(_subViewportContainer.Material, "shader_parameter/rotation_y", flipMultiplier * 90.0f, 0.5f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.In)
			.AsRelative();
		yRotationTween
			.TweenProperty(_subViewportContainer.Material, "shader_parameter/rotation_y", flipMultiplier * -90.0f, 0f)
			.Finished += () => _cardInstance.FlipCard();
		yRotationTween
			.TweenProperty(_subViewportContainer.Material, "shader_parameter/rotation_y", flipMultiplier * 90.0f, 0.5f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out)
			.AsRelative();

		Tween slightRotationTween = CreateTween();
		slightRotationTween
			.TweenProperty(_subViewportContainer, "rotation_degrees", flipMultiplier * 25.0f, 0.6f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.In)
			.AsRelative();
		slightRotationTween
			.TweenProperty(_subViewportContainer, "rotation_degrees", -flipMultiplier * 25.0f, 0.4f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out)
			.AsRelative();
	}

	public void OnFlipRight()
	{
		DoFlip(true);
	}

	public void OnFlipLeft()
	{
		DoFlip(false);
	}

	public void OnShake(float degrees, float duration)
	{
		DoShake(degrees, duration);
	}

	public void OnAnimateCardEffects(float delay = 1f)
	{
		Tween tween = CreateTween();
		tween.TweenCallback(Callable.From(ShowEffects))
			.SetDelay(delay);
	}

	public void OnScaleCard(float xScale, float yScale, float duration)
	{
		Tween tween = CreateTween();
		tween.TweenProperty(this, "scale", new Vector2(xScale, yScale), duration)
			.SetDelay(1.3f)
			.SetTrans(Tween.TransitionType.Elastic)
			.SetEase(Tween.EaseType.Out);
	}

	public void OnExit(float xPosition, float yPosition, float duration)
	{
		Tween tween = CreateTween();
		tween.TweenProperty(this, "position", new Vector2(xPosition, yPosition), duration)
			.SetEase(Tween.EaseType.InOut)
			.SetTrans(Tween.TransitionType.Back);
	}
}
