using Godot;

[GlobalClass]
public partial class EffectResource : Resource
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
    Health,
    Armor,
    Attack
}
