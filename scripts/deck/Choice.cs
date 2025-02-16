using System;
using System.Collections.Generic;
using Godot;

public class Choice
{
    public string Text { get; private set; } = "The text of the choice, describing it in brief.";
    public string HoverText { get; private set; } = "The hover text of the choice, describing it in more detail.";
    public Effect[] Effects { get; private set; }
    // TODO : Linked choice (A Card to force follow up after this Choice selected)
    // Should get enqued into follow-up queue on Apply

    public Choice(string text = null, string hoverText = null, Effect[] effects = null)
    {
        if (text is not null)
            Text = text;
        if (hoverText is not null)
            HoverText = hoverText;
        if (effects is not null)
            Effects = effects;
    }

    public Choice(ChoiceResource choiceResource)
    {
        string text = choiceResource.Text;
        string hoverText = choiceResource.HoverText;
        EffectResource[] effectResources = choiceResource.Effects;

        // Convert array of effectResources to array of effects
        Effect[] effects = new Effect[effectResources.Length];
        for (int i = 0; i < effectResources.Length; i++)
        {
            if (effectResources[i] is OneTimeEffectResource oneTimeResource)
            {
                effects[i] = new OneTimeEffect(oneTimeResource);
                continue;
            }
            if (effectResources[i] is RepeatEffectResource repeatResource)
            {
                effects[i] = new RepeatEffect(repeatResource);
                continue;
            }
        }
        
        Text = text;
        HoverText = hoverText;
        Effects = effects;
    }

    public void Apply()
    {
        foreach (Effect effect in Effects)
        {
            effect.Apply();
        }
    }

    public void PrintChoice()
    {
        GD.Print($"Text: {Text}");
        GD.Print($"Hover Text: {HoverText}");
        GD.Print($"Effects: {Effects.Length}");
        for (int i = 0; i < Effects.Length; i++)
        {
            GD.Print($"Effect #{i+1}:");
            Effects[i].PrintEffect();
        }
    }
}