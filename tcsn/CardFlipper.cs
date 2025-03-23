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
		_cardInstance.Connect("OnFlip", Callable.From(OnFlip));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnFlip()
	{
		Tween tween = CreateTween();
		tween
			.TweenProperty(this.Material, "shader_parameter/rotation_y", 90.0f, 0.5f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.In)
			.AsRelative();
		tween
			.TweenProperty(this.Material, "shader_parameter/rotation_y", -90.0f, 0f)
			.Finished += () => _cardInstance.FlipCard();
		tween
			.TweenProperty(this.Material, "shader_parameter/rotation_y", 90.0f, 0.5f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out)
			.AsRelative();
	}
}
