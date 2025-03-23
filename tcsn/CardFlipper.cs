using Godot;
using System;

public partial class CardFlipper : SubViewportContainer
{
	[Export] public NodePath CardInstance { get; set; }

	private CardInstance _cardInstance;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_cardInstance = NodeUtils.FindNodeWithError<CardInstance>(this, CardInstance, "CardInstance");
		_cardInstance.Connect("OnFlipRight", Callable.From(OnFlipRight));
		_cardInstance.Connect("OnFlipLeft", Callable.From(OnFlipLeft));
		_cardInstance.Connect("OnShake", Callable.From((float degrees, float duration) => OnShake(degrees, duration)));
	}

	public void DoShake(float degrees=0.4f, float duration=0.05f)
	{
		Tween _rotationTween = CreateTween();
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

	public void DoFlip(bool isFlipRight = true)
	{
		int flipMultiplier = 1;
		if (!isFlipRight) flipMultiplier = -1;

		Tween xRotationTween = CreateTween();
		xRotationTween
			.TweenProperty(this.Material, "shader_parameter/rotation_x", 15.0f, 0.65f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.In)
			.AsRelative();
		xRotationTween
			.TweenProperty(this.Material, "shader_parameter/rotation_x", -15.0f, 0.35f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out)
			.AsRelative();

		Tween yRotationTween = CreateTween();
		yRotationTween
			.TweenProperty(this.Material, "shader_parameter/rotation_y", flipMultiplier * 90.0f, 0.5f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.In)
			.AsRelative();
		yRotationTween
			.TweenProperty(this.Material, "shader_parameter/rotation_y", flipMultiplier * -90.0f, 0f)
			.Finished += () => _cardInstance.FlipCard();
		yRotationTween
			.TweenProperty(this.Material, "shader_parameter/rotation_y", flipMultiplier * 90.0f, 0.5f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out)
			.AsRelative();

		Tween slightRotationTween = CreateTween();
		slightRotationTween
			.TweenProperty(this, "rotation_degrees", -25.0f, 0.6f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.In)
			.AsRelative();
		slightRotationTween
			.TweenProperty(this, "rotation_degrees", 25.0f, 0.4f)
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
}
