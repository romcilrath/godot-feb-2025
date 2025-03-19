using System;
using System.ComponentModel;
using Godot;

public static class EffectFactory
{
    public static Effect CreateEffect(EffectResource effectResource)
    {
        if (effectResource is OneTimeStatEffectResource oneTimeResource)
        {
            return new OneStatTimeEffect(oneTimeResource);
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
}