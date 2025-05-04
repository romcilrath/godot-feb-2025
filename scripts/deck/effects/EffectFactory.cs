using System;
using System.ComponentModel;
using Godot;

public static class EffectFactory
{
    public static Effect CreateEffect(EffectResource effectResource)
    {
        if (effectResource is OneTimeStatEffectResource oneTimeResource)
        {
            return new OneTimeStatEffect(oneTimeResource);
        }
        else if (effectResource is RepeatStatEffectResource repeatResource)
        {
            return new RepeatStatEffect(repeatResource);
        }
        else if (effectResource is null)
        {
            return null;
        }

        throw new WarningException($"Unsupported EffectResource type: {effectResource?.GetType().Name}", nameof(effectResource));
    }

    public static Effect[] CreateEffects(EffectResource[] effectResources)
    {
        Effect[] effects = new Effect[effectResources.Length];
        for (int i = 0; i < effects.Length; i++)
            effects[i] = CreateEffect(effectResources[i]);
        return effects;
    }
}