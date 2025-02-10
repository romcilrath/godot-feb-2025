using System.Collections.Generic;

public class Choice
{
    public List<Effect> Effects { get; private set; } = new List<Effect>();

    public Choice(List<Effect> effects = null)
    {
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