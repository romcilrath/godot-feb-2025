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
        
        _isInitialPositionSet = false;
		_shadowRect.Visible = false;
	}

    public void SetInitialPosition ()
    {
        this._initialPosition = _choiceRect.Position;
        this._isInitialPositionSet = true;
        GD.Print("!!");
    }

	public override void _Process(double delta)
	{
        if (_isInitialPositionSet){
            if (_isHovered)
            {
                _lerpProgress = Mathf.Min(_lerpProgress + (float)(delta / _lerpDuration), 1.0f);
                _shadowLerpProgress = Mathf.Min(_shadowLerpProgress + (float)(delta / _shadowLerpDuration), 0.6f);
            }
            else
            {
                _lerpProgress = Mathf.Max(_lerpProgress - (float)(delta / _lerpDuration), 0.0f);
                _shadowLerpProgress = Mathf.Max(_shadowLerpProgress - (float)(delta / _shadowLerpDuration), 0.0f);
            }

            _choiceRect.Position = _initialPosition.Lerp(_initialPosition + _hoverOffset, _lerpProgress);
            _shadowRect.Modulate = new Color(_shadowRect.Modulate.R, _shadowRect.Modulate.G, _shadowRect.Modulate.B, _shadowLerpProgress);
            //_shadow.Position = _initialPosition.Lerp(_initialPosition - _hoverOffset, _lerpProgress);
        }
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
	}

	private void OnMouseExit() {
		_isHovered = false;
		if (_shadowLerpProgress == 0.0f) _shadowRect.Visible = false;
	}
}
