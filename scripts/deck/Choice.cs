using System.Collections.Generic;
using Godot;

public class Choice
{
    public string Text { get; private set; } = "The text of the choice, describing it in brief.";
    public string HoverText { get; private set; } = "The hover text of the choice, describing it in more detail.";
    public List<Effect> Effects { get; private set; } = new List<Effect>();

    public Choice(string text = null, string hoverText = null, List<Effect> effects = null)
    {
        if (text is not null)
            Text = text;
        if (hoverText is not null)
            HoverText = hoverText;
        if (effects is not null)
            Effects = effects;
    }

    public void Apply()
    {
        foreach (Effect effect in Effects)
        {
            effect.Apply();
        }
    }
}