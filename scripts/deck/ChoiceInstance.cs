using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

public partial class ChoiceInstance : ColorRect
{
    // The ChoiceResource and base Choice that this ChoiceInstance is displaying
	[Export] public ChoiceResource choiceResource;
	private Choice _choice;

    // These are the paths to the child nodes of the ChoiceInstance
    // Allows us tomake ChoiceInstance a resource and set the paths in the editor
	[Export] public NodePath ChoiceRectPath { get; set; }
	[Export] public NodePath TextPath { get; set; }
	[Export] public NodePath EffectRowPath { get; set; }

    // These are the child nodes of the ChoiceInstance
	private NinePatchRect _choiceRect;
	private RichTextLabel _text;
	private HBoxContainer _effectRow;

    // Detect if this choice/ a sibling choice has been selected already
    private bool _isDisabled = false; 

    // Define a signals
    [Signal]
    public delegate void OnShakeEventHandler(float degrees=0.4f, float duration=0.5f);
    [Signal]
    public delegate void ChoiceSelectedEventHandler();
    [Signal]
    public delegate void DismissCardEventHandler();

    // Hover variables
    private bool _isHovered = false;
    private bool _isInitialPositionSet = false;
	private Vector2 _initialPosition;
	private Vector2 _hoverOffset = new Vector2(0, -45); 
    private Tween _hoverPositionTween;
    private Tween _rotationTween;
	private Tween _sizeTween; 

	public override void _Ready()
	{
        // Find the child nodes
		_choiceRect = NodeUtils.FindNodeWithError<NinePatchRect>(this, ChoiceRectPath, "ChoiceRect");
		_text = NodeUtils.FindNodeWithError<RichTextLabel>(this, TextPath, "Text");
		_effectRow = NodeUtils.FindNodeWithError<HBoxContainer>(this, EffectRowPath, "EffectRow");

		// Ensure the material is unique, otherwise the shader will be shared between all instances
		if (_choiceRect.Material is ShaderMaterial shaderMaterial)
		{
			_choiceRect.Material = shaderMaterial.Duplicate() as ShaderMaterial;
		}

		// If choiceResource is defined via editor (like for debug) then SetChoice and LoadChoice
        if (this.choiceResource is not null)
        {
            SetChoice(new Choice(this.choiceResource));
            LoadChoice();
        }
	}    

    public void SetChoice(ChoiceResource newChoiceResource)
    {
		// Instiate the Choice object from the ChoiceResource
        SetChoice(new Choice(newChoiceResource));
    }

	public void SetChoice(Choice newChoice)
	{
        // Set the new Choice directly
		this._choice = newChoice;
	}

	public void LoadChoice()
	{
        // Set the choice text
		_text.Text = this._choice.Text;
        
        // No need to spawn icon instances if we hav eno effects
        if (this._choice.Effects is null) return;
        if (this._choice.Effects.Length == 0) return;
        
        // Spawn icon instances
        foreach (Effect effect in this._choice.Effects)
        {
            IconInstance iconInstance = GlobalReferences.Instance.IconInstanceScene.Instantiate() as IconInstance;
            _effectRow.AddChild(iconInstance);
            iconInstance.SetEffect(effect);

			ReferenceRect space = new ReferenceRect();
			space.CustomMinimumSize = new Vector2(50, 0);
			_effectRow.AddChild(space);
        }
        ReferenceRect finalSpace = new ReferenceRect();
        finalSpace.CustomMinimumSize = new Vector2(50, 0);
        _effectRow.AddChild(finalSpace);
	}
    
    public void SetDisabled(bool isDisabled=true)
    {
        _isDisabled = isDisabled;
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

        if (_isHovered && !_isDisabled)
        {
            float angleXMax = 2.0f, angleYMax = 2.0f;
            float time = (float)Time.GetTicksMsec() / 3000.0f; // Time in seconds

            if (_choiceRect.Material is ShaderMaterial choiceShaderMaterial)
                UpdateShaderRotation(choiceShaderMaterial, _choiceRect.Position + (_choiceRect.Size / 2), angleXMax, angleYMax, time);
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
        }
    }

    private void KillTweens() 
    {
        // Kill our tweens
        _hoverPositionTween?.Kill();
        _sizeTween?.Kill();
        _rotationTween?.Kill();
    }

	private void OnMouseEnter() {
        if (_isDisabled) return;

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
        _rotationTween = CreateTween();
        _rotationTween
            .TweenProperty(_choiceRect, "rotation_degrees", 1, 0.05f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);
        _rotationTween
            .Chain()
            .TweenProperty(_choiceRect, "rotation_degrees", -1, 0.05f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);
        _rotationTween
            .Chain()
            .TweenProperty(_choiceRect, "rotation_degrees", 0, 0.05f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);

        // Tween the choice rect to the hover offset position
        _hoverPositionTween = CreateTween();
        _hoverPositionTween.TweenProperty(_choiceRect, "position", _initialPosition + _hoverOffset, 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);

        // Tween the choice rect to scale up slightly
        _sizeTween = CreateTween();
        _sizeTween.TweenProperty(_choiceRect, "scale", new Vector2(1.05f, 1.05f), 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);
	}

	private void OnMouseExit() {
        // Reset the hover flag
		_isHovered = false;
        
        // Kill any existing tween before starting a new one
        KillTweens();

        // Tween the choice rect to rotate back to 0
        _rotationTween = CreateTween();
        _rotationTween.TweenProperty(_choiceRect, "rotation_degrees", 0, 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);

        // Tween the choice rect back to the initial position
        _hoverPositionTween = CreateTween();
        _hoverPositionTween.TweenProperty(_choiceRect, "position", _initialPosition, 0.1f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);

        // Tween the choice rect back to its original size
        _sizeTween = CreateTween();
        _sizeTween.TweenProperty(_choiceRect, "scale", new Vector2(1.0f, 1.0f), 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);
	}

    private async void DelayApplyCard(int delay = 500)
    {
        await Task.Delay(delay); // Delay for 1 second
        _choice.Apply();
        GameManager.Instance.IncrementTurn();
        EmitSignal(SignalName.DismissCard);
        EmitSignal(SignalName.ChoiceSelected);
    }


    private void OnChoiceClicked(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            if (_isDisabled) return;
            _isDisabled = true;

            GD.Print($"Choice clicked: {_choice?.Text}");
            EmitSignal(SignalName.OnShake, 2f, 0.05f);

            KillTweens();

            // Tween the choice rect to rotate back to 0
            _rotationTween = CreateTween();
            _rotationTween.TweenProperty(_choiceRect, "rotation_degrees", 0, 0.4f)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Elastic);

            // Tween the choice rect back to the initial position
            _hoverPositionTween = CreateTween();
            _hoverPositionTween.TweenProperty(_choiceRect, "position", _initialPosition, 0.05f)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Bounce);

            // Expand then tween the choice rect back to its original size
            _sizeTween = CreateTween();
            _sizeTween.TweenProperty(_choiceRect, "scale", new Vector2(0.8f, 1.2f), 0.2f)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Elastic);
            _sizeTween.Chain()
                .TweenProperty(_choiceRect, "scale", new Vector2(1.2f, 1.0f), 0.2f)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Elastic);
            _sizeTween.Chain()
                .TweenProperty(_choiceRect, "scale", new Vector2(1.0f, 1.0f), 0.2f)
                .SetEase(Tween.EaseType.In)
                .SetTrans(Tween.TransitionType.Bounce);

            DelayApplyCard();
        }
    }
}
