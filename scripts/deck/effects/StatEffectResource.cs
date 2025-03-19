using Godot;

[GlobalClass]
public partial class StatEffectResource : EffectResource
{
    #region VARIABLES

    [Export] public TargetType Target;
    [Export] public float Value;
    [Export] public ActionType ActionType;

    #endregion
}

public enum TargetType
{
    Money,
    Vitality,
    Grit,
    Rations
}
