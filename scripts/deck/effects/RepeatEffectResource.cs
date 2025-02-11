using Godot;

[GlobalClass]
public partial class RepeatEffectResource : EffectResource
{
    #region VARIABLES
    
    [Export] public int EveryTurn;
    [Export] public int ForTurns;

    #endregion
}
