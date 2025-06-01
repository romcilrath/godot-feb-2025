using Godot;
using System;

public partial class HoverController : Control
{
	[Export] public NodePath TargetPath { get; set; }

	[Export] public Vector2 hoverPositionOffset = new Vector2(0, 0); 
	[Export] public Vector2 hoverScale = new Vector2(1.05f, 1.05f);
	[Export] public Vector2 hoverShadowOffset = new Vector2(0, 0);
	
	private Control _target;

	private bool _isEnabled = false;
	private bool _isHovered = false;

	private bool _isInitialPositionSet = false;
	private bool _isInitialScaleSet = false;
	private bool _isInitialShadowOffsetSet = false;
	private Vector2 _initialScale;
	private Vector2 _initialPosition;
	private Vector2 _initialShadowOffset;

	private Tween _hoverPositionTween;
	private Tween _rotationTween;
	private Tween _scaleTween; 
	private Tween _shadowOffsetTween;

	public override void _Ready()
	{
		// Find the child nodes
		_target = NodeUtils.FindNodeWithError<Control>(this, TargetPath, "Control");

		// Ensure the material is unique, otherwise the shader will be shared between all instances
		if (_target.Material is ShaderMaterial shaderMaterial)
		{
			_target.Material = shaderMaterial.Duplicate() as ShaderMaterial;
		}
	}   

	public override void _Process(double delta)
	{
		void UpdateShaderRotation(ShaderMaterial material, Vector2 anchorCenter, float angleXMax, float angleYMax, float time)
		{
			// Calculate circular motion
			float angle = Mathf.Pi * 2 * time; // Full circle over time
			float rotX = Mathf.Sin(angle) * angleXMax;
			float rotY = Mathf.Cos(angle) * angleYMax;

			material.Set("shader_parameter/rotation_x", rotX);
			material.Set("shader_parameter/rotation_y", rotY);
		}

		void UpdateShaderShadowOffset(ShaderMaterial material, Vector2 offset)
		{
			material.Set("shader_parameter/offset", offset);
		}

		if (_isHovered && _isEnabled)
		{
			float angleXMax = 2.0f, angleYMax = 2.0f;
			float time = (float)Time.GetTicksMsec() / 3000.0f; // Time in seconds

			if (_target.Material is ShaderMaterial cardShaderMaterial)
			{
				UpdateShaderRotation(cardShaderMaterial, _target.Position + (_target.Size / 2), angleXMax, angleYMax, time);
				UpdateShaderShadowOffset(cardShaderMaterial, _initialShadowOffset + hoverShadowOffset);
			}
		}
		else
		{
			void ResetShaderRotation(ShaderMaterial material)
			{
				material.Set("shader_parameter/rotation_x", 0.0f);
				material.Set("shader_parameter/rotation_y", 0.0f);
			}

			void ResetShaderShadowOffset(ShaderMaterial material)
			{
				material.Set("shader_parameter/offset", _initialShadowOffset);
			}

			if (_target.Material is ShaderMaterial cardShaderMaterial)
			{
				ResetShaderRotation(cardShaderMaterial);
				ResetShaderShadowOffset(cardShaderMaterial);
			}
		}
	}

	public void SetEnabled(bool isEnabled)
	{
		this._isEnabled = isEnabled;

		if (this._isEnabled && this._isHovered)
			OnMouseEnter();
	}

	public void SetInitialScale ()
	{
		// Save the initial scale of the choice rect
		this._initialScale = _target.Scale;
		this._isInitialScaleSet = true;
	}

	public void SetInitialPosition ()
	{
		// Save the initial position of the choice rect
		this._initialPosition = _target.Position;
		this._isInitialPositionSet = true;
	}

	private void SetInitialShadowOffset ()
	{
		// Set the initial shadow offset
		if (_target.Material is ShaderMaterial shaderMaterial)
		{
			this._initialShadowOffset = (Vector2)shaderMaterial.Get("shader_parameter/offset");
		}
	}

	private void KillTweens() 
	{
		// Kill our tweens
		_hoverPositionTween?.Kill();
		_scaleTween?.Kill();
		_rotationTween?.Kill();
		_shadowOffsetTween?.Kill();
	}

	private void OnMouseEnter() {
		// Set the hover flag
		_isHovered = true;

		// If hover is disabled, do nothing
		if (!_isEnabled) return;

		// Ensure initial scale, position, and shadow offset are set before applying hover effects
		if (!_isInitialScaleSet) SetInitialScale();
		if (!_isInitialPositionSet) SetInitialPosition();
		if (!_isInitialShadowOffsetSet) SetInitialShadowOffset();

		// Ensure shader perspective parameters are reset on hover
		if (_target.Material is ShaderMaterial shaderMaterial)
		{
			shaderMaterial.Set("shader_parameter/x_rotation", 0.0f); 
			shaderMaterial.Set("shader_parameter/y_rotation", 0.0f); 

			shaderMaterial.Set("shader_parameter/offset", _initialShadowOffset);
		}
			
		// Kill any existing tween before starting a new one
		KillTweens();
		
		// Tween the choice rect to rotate slightly
		_rotationTween = CreateTween();
		_rotationTween
			.TweenProperty(_target, "rotation_degrees", 3, 0.0)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Elastic);
		_rotationTween
			.Chain()
			.TweenProperty(_target, "rotation_degrees", -3, 0.05f)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Elastic);
		_rotationTween
			.Chain()
			.TweenProperty(_target, "rotation_degrees", 0, 0.05f)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Elastic);

		// Tween the choice rect to the hover offset position
		_hoverPositionTween = CreateTween();
		_hoverPositionTween.TweenProperty(_target, "position", _initialPosition + hoverPositionOffset, 0.4f)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Elastic);

		// Tween the choice rect to scale up slightly
		_scaleTween = CreateTween();
		_scaleTween.TweenProperty(_target, "scale", hoverScale * _initialScale, 0.4f)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Elastic);
	}

	private void OnMouseExit() {
		// Reset the hover flag
		_isHovered = false;
		
		// If hover is disabled, do nothing
		if (!_isEnabled) return;
		
		// Kill any existing tween before starting a new one
		KillTweens();

		// Tween the choice rect to rotate back to 0
		_rotationTween = CreateTween();
		_rotationTween.TweenProperty(_target, "rotation_degrees", 0, 0.4f)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Elastic);

		// Tween the choice rect back to the initial position
		_hoverPositionTween = CreateTween();
		_hoverPositionTween.TweenProperty(_target, "position", _initialPosition, 0.1f)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Elastic);

		// Tween the choice rect back to its original scale
		_scaleTween = CreateTween();
		_scaleTween.TweenProperty(_target, "scale", _initialScale, 0.4f)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Elastic);
	}
}