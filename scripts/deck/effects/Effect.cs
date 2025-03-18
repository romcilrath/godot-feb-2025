using System;
using Godot;

public abstract class Effect
{
    public Effect()
    {
    }

    public Effect(EffectResource effectResource)
    {
    }

    public abstract void Apply();

    public abstract void PrintEffect();
}