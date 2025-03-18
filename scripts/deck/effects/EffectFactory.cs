using System;
using System.ComponentModel;
using Godot;

public static class EffectFactory
{
    public static Effect CreateEffect(EffectResource effectResource)
    {
        if (effectResource is OneTimeEffectResource oneTimeResource)
        {
            return new OneTimeEffect(oneTimeResource);
        }
        else if (effectResource is RepeatEffectResource repeatResource)
        {
            return new RepeatEffect(repeatResource);
        }
        else if (effectResource is null)
        {
            return null;
        }

        throw new WarningException($"Unsupported EffectResource type: {effectResource?.GetType().Name}", nameof(effectResource));
    }
}