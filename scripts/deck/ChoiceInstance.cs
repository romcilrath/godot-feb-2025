using Godot;
using System;
using System.Diagnostics;

public partial class ChoiceInstance : ColorRect
{
	[Export] public ChoiceResource choiceResource;
	private Choice _choice;

	[Export] public NodePath ChoiceRectPath { get; set; }
	[Export] public NodePath ShadowRectPath { get; set; }
	[Export] public NodePath TextPath { get; set; }

	private NinePatchRect _choiceRect;
	private NinePatchRect _shadowRect;
	private RichTextLabel _text;

    private bool _isHovered = false;

    private bool _isInitialPositionSet;
	private Vector2 _initialPosition;
	private Vector2 _hoverOffset = new Vector2(45, -45); // Adjust offset as needed
	private float _lerpDuration = 0.1f;
	private float _lerpProgress = 0.0f;
	private float _shadowLerpProgress = 0.0f;
	private float _shadowLerpDuration = 0.1f;

	public override void _Ready()
	{
		_choiceRect = NodeUtils.FindNodeWithError<NinePatchRect>(this, ChoiceRectPath, "ChoiceRect");
		_shadowRect = NodeUtils.FindNodeWithError<NinePatchRect>(this, ShadowRectPath, "ShadowRect");
		_text = NodeUtils.FindNodeWithError<RichTextLabel>(this, TextPath, "Text");

		// Ensure the material is unique
		if (_choiceRect.Material is ShaderMaterial shaderMaterial)
		{
			_choiceRect.Material = shaderMaterial.Duplicate() as ShaderMaterial;
		}

		_isInitialPositionSet = false;
		_shadowRect.Visible = false;
	}

    public void SetInitialPosition ()
    {
        this._initialPosition = _choiceRect.Position;
        this._isInitialPositionSet = true;
    }

    public override void _Process(double delta)
    {
        if (!_isInitialPositionSet) return;

        _lerpProgress = Mathf.Clamp(_lerpProgress + (float)(delta / _lerpDuration) * (_isHovered ? 1 : -1), 0.0f, 1.0f);
        _shadowLerpProgress = Mathf.Clamp(_shadowLerpProgress + (float)(delta / _shadowLerpDuration) * (_isHovered ? 1 : -1), 0.0f, 0.6f);

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
            float angleXMax = 1.5f, angleYMax = 1.5f;

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

        _choiceRect.Position = _initialPosition.Lerp(_initialPosition + _hoverOffset, _lerpProgress);
        _shadowRect.Modulate = new Color(_shadowRect.Modulate.R, _shadowRect.Modulate.G, _shadowRect.Modulate.B, _shadowLerpProgress);
        _shadowRect.Position = _initialPosition.Lerp(_initialPosition - _hoverOffset, _lerpProgress);
    }

	public void LoadChoice()
	{
		Choice choice = new Choice(choiceResource);
		this._choice = choice;

		_text.Text = this._choice.Text;
	}

	public void SetChoiceResource(ChoiceResource newChoiceResource)
	{
		choiceResource = newChoiceResource;
		LoadChoice();
	}

	private void OnMouseEnter() {
        _isHovered = true;
        if (_isInitialPositionSet == false) SetInitialPosition();
		_shadowRect.Visible = true;

        // Ensure shader parameters are reset on hover
        if (_choiceRect.Material is ShaderMaterial shaderMaterial)
        {
            shaderMaterial.Set("shader_parameter/x_rotation", 0.0f); // Updated parameter name
            shaderMaterial.Set("shader_parameter/y_rotation", 0.0f); // Updated parameter name
        }
	}

	private void OnMouseExit() {
		_isHovered = false;
		if (_shadowLerpProgress == 0.0f) _shadowRect.Visible = false;
	}
}
