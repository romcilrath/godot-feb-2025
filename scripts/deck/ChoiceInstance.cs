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

	public override void _Ready()
	{
		_shadow = GetNode<NinePatchRect>(ShadowPath);
        if (_shadow == null)
        {
            GD.PrintErr($"Shadow node not found at path: {ShadowPath}");
        }

        _text = GetNode<RichTextLabel>(TextPath);
        if (_text == null)
        {
            GD.PrintErr($"Text node not found at path: {TextPath}");
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
}
