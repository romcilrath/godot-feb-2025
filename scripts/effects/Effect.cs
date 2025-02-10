using System;

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

    public abstract void Apply();
}