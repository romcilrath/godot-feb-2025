using Godot;
using System;

public partial class CardBack : NinePatchRect
{
	private bool _isHovered = false;
	private bool _isEnabled = true;

    [Signal]
    public delegate void OnCardBackSelectedEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	
    private void OnGUIInput(InputEvent @event)
	{
		if (!_isHovered) return;
		if (!_isEnabled) return;

        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			EmitSignal(SignalName.OnCardBackSelected);
			this._isEnabled = false;
		}
	}

	private void OnMouseEnter() {
        // Set the hover flag
        _isHovered = true;
	}

	private void OnMouseExit() {
        // Set the hover flag
        _isHovered = false;
	}
}
