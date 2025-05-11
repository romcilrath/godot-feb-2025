using Godot;
using System;

public partial class TooltipPanel : PanelContainer
{
	[Export] public RichTextLabel Label;

	public void SetTooltipText(string text)
	{
		Label.Text = "[center]" + text + "[/center]";
	}
}
