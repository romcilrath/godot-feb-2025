using System;
using Godot;

public enum ActionType
{
    Set, Add, Multiply
}

public abstract class StatEffect: Effect
{
    public Stat Target { get; private set; }
    public float Value { get; private set; }
    public ActionType ActionType { get; private set; }

    public StatEffect(Stat target, float value, ActionType actionType = ActionType.Add)
    {
        Target = target;
        Value = value;
        ActionType = actionType;
    }

    public StatEffect(StatEffectResource statEffectResource)
    {
        TargetType targetType = statEffectResource.Target;
        float value = statEffectResource.Value;
        ActionType actionType = statEffectResource.ActionType;
        
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

    public override abstract void Apply();

    public override void PrintEffect()
    {
        GD.Print($"Target: {Target.Name}");
        GD.Print($"Value: {Value}");
        GD.Print($"ActionType: {ActionType}");
    }
}