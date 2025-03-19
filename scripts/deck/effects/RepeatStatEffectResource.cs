using Godot;

[GlobalClass]
public partial class RepeatStatEffectResource : StatEffectResource
{
    #region VARIABLES
    
    [Export] public int EveryTurn;
    [Export] public int ForTurns;

    #endregion
}
