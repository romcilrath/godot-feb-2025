using System;
using Godot;

public class OneTimeEffect: Effect
{
    public OneTimeEffect(Stat target, float value, ActionType actionType = ActionType.Add)
        :base(target, value, actionType)
    {

    }

    public OneTimeEffect(OneTimeEffectResource oneTimeEffectResource)
        :base(oneTimeEffectResource)
    { 

    }

    public override void Apply()
    {
        switch (ActionType)
        {
            case ActionType.Set:
                Target.Set(Value);
                break;
            case ActionType.Add:
                Target.Add(Value);
                break;
            case ActionType.Multiply:
                Target.Multiply(Value);
                break;
            default:
                GD.Print($"Cannot apply OneTimeEfect with ActionType: {ActionType}");
                break;
        }
    }
}