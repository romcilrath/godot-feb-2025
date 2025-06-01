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
    [Export] public NodePath HoverControllerPath { get; set; }

    [Export] public PackedScene _tooltipScene;

    // These are the child nodes of the ChoiceInstance
	private NinePatchRect _choiceRect;
	private RichTextLabel _text;
	private HBoxContainer _effectRow;
    private HoverController _hoverController;

    // Detect if this choice/ a sibling choice has been selected already
    private bool _isDisabled = false; 

    // Define a signals
    [Signal]
    public delegate void OnShakeEventHandler(float degrees=0.4f, float duration=0.5f);
    [Signal]
    public delegate void OnChoiceSelectedEventHandler();

	public override void _Ready()
	{
        // Find the child nodes
		_choiceRect = NodeUtils.FindNodeWithError<NinePatchRect>(this, ChoiceRectPath, "ChoiceRect");
		_text = NodeUtils.FindNodeWithError<RichTextLabel>(this, TextPath, "Text");
		_effectRow = NodeUtils.FindNodeWithError<HBoxContainer>(this, EffectRowPath, "EffectRow");
        _hoverController = NodeUtils.FindNodeWithError<HoverController>(this, HoverControllerPath, "HoverController");
        
		// If choiceResource is defined via editor (like for debug) then SetChoice and LoadChoice
        if (this.choiceResource is not null)
        {
            SetChoice(new Choice(this.choiceResource));
            LoadChoice();
        }

        // Initialize HoverController
        _hoverController.SetEnabled(!_isDisabled);
	}   

    public void SetColor(Color color)
    {
        // Set the color of the ChoiceRect
        _choiceRect.SelfModulate = color;
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
        // Set the choice text and hover text
		_text.Text = this._choice.Text;
        TooltipText = this._choice.HoverText;
        
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
        _hoverController.SetEnabled(!_isDisabled);
    }

    private async void DelayApplyCard(int delay = 1000)
    {
        await Task.Delay(delay); 
        _choice.Apply();
    }


    private void OnChoiceClicked(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            if (_isDisabled) return;
            SetDisabled(true);

            GD.Print($"Choice clicked: {_choice?.Text}");
            EmitSignal(SignalName.OnShake, 2f, 0.05f);
            
            _hoverController.SetEnabled(false);

            GameManager.Instance.DelayIncrementTurn(1, 500);
            DelayApplyCard(500);
            EmitSignal(SignalName.OnChoiceSelected);

            // Disable the tooltip by setting it to an empty string
            TooltipText = "";
        }
    }

    public override Control _MakeCustomTooltip(string forText)
    {
        TooltipPanel tooltip = (TooltipPanel)_tooltipScene.Instantiate();
        tooltip.SetTooltipText(forText);
        return tooltip;
    }
}
