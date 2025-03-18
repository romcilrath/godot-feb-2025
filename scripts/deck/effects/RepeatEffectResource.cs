using Godot;

[GlobalClass]
public partial class RepeatEffectResource : StatEffectResource
{
    #region VARIABLES
    
    [Export] public int EveryTurn;
    [Export] public int ForTurns;

    #endregion
}
