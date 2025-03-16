using Godot;
using System;
using System.Diagnostics;

public partial class ChoiceInstance : ColorRect
{
    // The ChoiceResource and base Choice that this ChoiceInstance is displaying
	[Export] public ChoiceResource choiceResource;
	private Choice _choice;

    // These are the paths to the child nodes of the ChoiceInstance
    // Allows us tomake ChoiceInstance a resource and set the paths in the editor
	[Export] public NodePath ChoiceRectPath { get; set; }
	[Export] public NodePath ShadowRectPath { get; set; }
	[Export] public NodePath TextPath { get; set; }

    // These are the child nodes of the ChoiceInstance
	private NinePatchRect _choiceRect;
	private NinePatchRect _shadowRect;
	private RichTextLabel _text;

    // Hover variables
    private bool _isHovered = false;
    private bool _isInitialPositionSet = false;
	private Vector2 _initialPosition;
	private Vector2 _hoverOffset = new Vector2(0, -45); 
	private Vector2 _shadowInitialPosition;
    private Tween hoverPositionTween;
    private Tween rotationTween;
	private Tween sizeTween; 
	private Tween shadowSizeTween;
    private Tween shadowHoverPositionTween;

	public override void _Ready()
	{
        // Find the child nodes
		_choiceRect = NodeUtils.FindNodeWithError<NinePatchRect>(this, ChoiceRectPath, "ChoiceRect");
		_shadowRect = NodeUtils.FindNodeWithError<NinePatchRect>(this, ShadowRectPath, "ShadowRect");
		_text = NodeUtils.FindNodeWithError<RichTextLabel>(this, TextPath, "Text");

		// Ensure the material is unique, otherwise the shader will be shared between all instances
		if (_choiceRect.Material is ShaderMaterial shaderMaterial)
		{
			_choiceRect.Material = shaderMaterial.Duplicate() as ShaderMaterial;
		}

        // Ensure the material of _shadowRect is unique
        if (_shadowRect.Material is ShaderMaterial shadowShaderMaterial)
        {
            _shadowRect.Material = shadowShaderMaterial.Duplicate() as ShaderMaterial;
        }
	}    

	public void LoadChoice()
	{
        // Convert the choiceResource to a Choice object
		Choice choice = new Choice(choiceResource);
		this._choice = choice;

        // Set the choice text
		_text.Text = this._choice.Text;
	}

	public void SetChoiceResource(ChoiceResource newChoiceResource)
	{
        // Set the new choice resource and load the choice
		choiceResource = newChoiceResource;
		LoadChoice();
	}

    public void SetInitialPosition ()
    {
        // Save the initial position of the choice rect
        this._initialPosition = _choiceRect.Position;
        this._shadowInitialPosition = _shadowRect.Position; // Save the initial position of the shadow rect
        this._isInitialPositionSet = true;

        // Set the pivot offset to the center of the choice rect
        _choiceRect.PivotOffset = _choiceRect.Size / 2;
    }

    public override void _Process(double delta)
    {
        // If the initial position hasn't been set, hold off
        if (!_isInitialPositionSet) SetInitialPosition();

        void UpdateShaderRotation(ShaderMaterial material, Vector2 anchorCenter, float angleXMax, float angleYMax, float time)
        {
            // Calculate circular motion
            float angle = Mathf.Pi * 2 * time; // Full circle over time
            float rotX = Mathf.Sin(angle) * angleXMax;
            float rotY = Mathf.Cos(angle) * angleYMax;

            material.Set("shader_parameter/rotation_x", rotX);
            material.Set("shader_parameter/rotation_y", rotY);
        }

        if (_isHovered)
        {
            float angleXMax = 2.0f, angleYMax = 2.0f;
            float time = (float)Time.GetTicksMsec() / 3000.0f; // Time in seconds

            if (_choiceRect.Material is ShaderMaterial choiceShaderMaterial)
                UpdateShaderRotation(choiceShaderMaterial, _choiceRect.Position + (_choiceRect.Size / 2), angleXMax, angleYMax, time);

            if (_shadowRect.Material is ShaderMaterial shadowShaderMaterial)
                UpdateShaderRotation(shadowShaderMaterial, _shadowRect.Position + (_shadowRect.Size / 2), angleXMax, angleYMax, time);
        }
        else
        {
            void ResetShaderRotation(ShaderMaterial material)
            {
                material.Set("shader_parameter/rotation_x", 0.0f);
                material.Set("shader_parameter/rotation_y", 0.0f);
            }

            if (_choiceRect.Material is ShaderMaterial choiceShaderMaterial)
                ResetShaderRotation(choiceShaderMaterial);

            if (_shadowRect.Material is ShaderMaterial shadowShaderMaterial)
                ResetShaderRotation(shadowShaderMaterial);
        }
    }

    private void KillTweens() 
    {
        // Kill our tweens
        hoverPositionTween?.Kill();
        sizeTween?.Kill();
        shadowSizeTween?.Kill();
        shadowHoverPositionTween?.Kill();
        rotationTween?.Kill();
    }

	private void OnMouseEnter() {
        // Set the hover flag
        _isHovered = true;

        // Ensure shader perspective parameters are reset on hover
        if (_choiceRect.Material is ShaderMaterial shaderMaterial)
        {
            shaderMaterial.Set("shader_parameter/x_rotation", 0.0f); 
            shaderMaterial.Set("shader_parameter/y_rotation", 0.0f); 
        }
            
        // Kill any existing tween before starting a new one
        KillTweens();
        
        // Tween the choice rect to rotate slightly
        rotationTween = CreateTween();
        rotationTween
            .TweenProperty(_choiceRect, "rotation_degrees", 1, 0.05f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);
        rotationTween
            .Chain()
            .TweenProperty(_choiceRect, "rotation_degrees", -1, 0.05f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);
        rotationTween
            .Chain()
            .TweenProperty(_choiceRect, "rotation_degrees", 0, 0.05f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);

        // Tween the choice rect to the hover offset position
        hoverPositionTween = CreateTween();
        hoverPositionTween.TweenProperty(_choiceRect, "position", _initialPosition + _hoverOffset, 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);

        // Tween the choice rect to scale up slightly
        sizeTween = CreateTween();
        sizeTween.TweenProperty(_choiceRect, "scale", new Vector2(1.05f, 1.05f), 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);

        // Tween the shadow rect to the inverse hover offset position
        shadowHoverPositionTween = CreateTween();
        shadowHoverPositionTween.TweenProperty(_shadowRect, "position", _shadowInitialPosition - _hoverOffset, 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);

        // Tween the shadow rect to scale down slightly
        shadowSizeTween = CreateTween();
        shadowSizeTween.TweenProperty(_shadowRect, "scale", new Vector2(0.95f, 0.95f), 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);
	}

	private void OnMouseExit() {
        // Reset the hover flag
		_isHovered = false;
        
        // Kill any existing tween before starting a new one
        KillTweens();

        // Tween the choice rect to rotate back to 0
        rotationTween = CreateTween();
        rotationTween.TweenProperty(_choiceRect, "rotation_degrees", 0, 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);

        // Tween the choice rect back to the initial position
        hoverPositionTween = CreateTween();
        hoverPositionTween.TweenProperty(_choiceRect, "position", _initialPosition, 0.1f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);

        // Tween the choice rect back to its original size
        sizeTween = CreateTween();
        sizeTween.TweenProperty(_choiceRect, "scale", new Vector2(1.0f, 1.0f), 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);

        // Tween the shadow rect back to its original position
        shadowHoverPositionTween = CreateTween();
        shadowHoverPositionTween.TweenProperty(_shadowRect, "position", _shadowInitialPosition, 0.1f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);

        // Tween the shadow rect back to its original size
        shadowSizeTween = CreateTween();
        shadowSizeTween.TweenProperty(_shadowRect, "scale", new Vector2(1.0f, 1.0f), 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);
	}
}
