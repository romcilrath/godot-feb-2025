using System;
using Godot;

public enum ActionType
{
    Set, Add, Multiply
}

public abstract class Effect
{
    public Stat Target { get; private set; }
    public float Value { get; private set; }
    public ActionType ActionType { get; private set; }

    public Effect(Stat target, float value, ActionType actionType = ActionType.Add)
    {
        Target = target;
        Value = value;
        ActionType = actionType;
    }

    public Effect(EffectResource effectResource)
    {
        TargetType targetType = effectResource.Target;
        float value = effectResource.Value;
        ActionType actionType = effectResource.ActionType;
        
        Stat target = null;
        switch (targetType)
        {
            case TargetType.Money:
                target = PlayerManager.Instance.Money;
                break;
            case TargetType.Health:
                target = PlayerManager.Instance.Health;
                break;
            case TargetType.Armor:
                target = PlayerManager.Instance.Armor;
                break;
            case TargetType.Attack:
                target = PlayerManager.Instance.Attack;
                break;
            default:
                GD.Print($"Cannot load EffectResource with TargetType: {targetType}");
                break;
        }

        Target = target;
        Value = value;
        ActionType = actionType;
    }

    public abstract void Apply();

    public void PrintEffect()
    {
        GD.Print($"Target: {Target.Name}");
        GD.Print($"Value: {Value}");
        GD.Print($"ActionType: {ActionType}");
    }
}