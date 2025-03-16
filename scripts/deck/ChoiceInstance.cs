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
    private bool _isInitialPositionSet;
	private Vector2 _initialPosition;
	private Vector2 _hoverOffset = new Vector2(45, -45); 
    private Tween hoverPositionTween;
    private Tween rotationTween;

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

        // Ensure the material is unique
		_isInitialPositionSet = false;
		_shadowRect.Visible = false;
	}

    public void SetInitialPosition ()
    {
        // Save the initial position of the choice rect
        this._initialPosition = _choiceRect.Position;
        this._isInitialPositionSet = true;

        // Set the pivot offset to the center of the choice rect
        _choiceRect.PivotOffset = _choiceRect.Size / 2;
    }

    public override void _Process(double delta)
    {
        // If the initial position hasn't been set hold off
        if (!_isInitialPositionSet) return;

        void UpdateShaderRotation(ShaderMaterial material, Vector2 anchorCenter, Vector2 mousePos, float angleXMax, float angleYMax)
        {
            float lerpValX = Mathf.Clamp((mousePos.X - anchorCenter.X) / _choiceRect.Size.X, -0.5f, 0.5f) + 0.5f;
            float lerpValY = Mathf.Clamp((mousePos.Y - anchorCenter.Y) / _choiceRect.Size.Y, -0.5f, 0.5f) + 0.5f;

            float rotX = Mathf.Lerp(-angleXMax, angleXMax, lerpValX);
            float rotY = Mathf.Lerp(angleYMax, -angleYMax, lerpValY);

            material.Set("shader_parameter/rotation_x", float.IsNaN(rotX) ? 0.0f : rotX);
            material.Set("shader_parameter/rotation_y", float.IsNaN(rotY) ? 0.0f : rotY);
        }

        if (_isHovered)
        {
            Vector2 mousePos = _choiceRect.GetLocalMousePosition();
            Vector2 anchorCenter = _choiceRect.Position + (_choiceRect.Size / 2);
            float angleXMax = 2.0f, angleYMax = 2.0f;

            if (_choiceRect.Material is ShaderMaterial choiceShaderMaterial)
                UpdateShaderRotation(choiceShaderMaterial, anchorCenter, mousePos, angleXMax, angleYMax);

            if (_shadowRect.Material is ShaderMaterial shadowShaderMaterial)
                UpdateShaderRotation(shadowShaderMaterial, _shadowRect.Position + (_shadowRect.Size / 2), mousePos, angleXMax, angleYMax);
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

	private void OnMouseEnter() {
        // Set the initial position if it hasn't been set yet on first mouse hover
        if (_isInitialPositionSet == false) SetInitialPosition();

        // Set the hover offset and make the shadow visible
        _isHovered = true;
		_shadowRect.Visible = true;

        // Ensure shader perspective parameters are reset on hover
        if (_choiceRect.Material is ShaderMaterial shaderMaterial)
        {
            shaderMaterial.Set("shader_parameter/x_rotation", 0.0f); 
            shaderMaterial.Set("shader_parameter/y_rotation", 0.0f); 
        }
        
        // Kill any existing tween before starting a new one
        hoverPositionTween?.Kill();
        rotationTween?.Kill();

        // Tween the choice rect to the hover position
        hoverPositionTween = CreateTween();
        hoverPositionTween.TweenProperty(_choiceRect, "position", _initialPosition + _hoverOffset, 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);
        
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
	}

	private void OnMouseExit() {
        // Reset the hover offset and hide the shadow
		_isHovered = false;
        _shadowRect.Visible = false;
        
        // Kill any existing tween before starting a new one
        hoverPositionTween?.Kill();
        rotationTween?.Kill();

        // Tween the choice rect back to the initial position
        hoverPositionTween = CreateTween();
        hoverPositionTween.TweenProperty(_choiceRect, "position", _initialPosition, 0.1f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);

        // Tween the choice rect to rotate back to 0
        rotationTween = CreateTween();
        rotationTween.TweenProperty(_choiceRect, "rotation_degrees", 0, 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);
	}
}
