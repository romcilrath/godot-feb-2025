using Godot;
using System;
using System.Diagnostics;

public partial class ChoiceInstance : NinePatchRect
{
	[Export] public ChoiceResource choiceResource;
	private Choice _choice;

	[Export] public NodePath ShadowPath { get; set; }
	[Export] public NodePath TextPath { get; set; }

	private NinePatchRect _shadow;
	private RichTextLabel _text;

    private bool _isHovered = false;

	public override void _Ready()
	{
		_shadow = NodeUtils.FindNodeWithError<NinePatchRect>(this, ShadowPath, "Shadow");
		_text = NodeUtils.FindNodeWithError<RichTextLabel>(this, TextPath, "Text");
        
		_shadow.Visible = false;
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
	}

	private void OnMouseExit() {
		_isHovered = false;
	}
}
