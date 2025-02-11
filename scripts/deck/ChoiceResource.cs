using Godot;

[GlobalClass]
public partial class ChoiceResource : Resource
{
    #region VARIABLES

    [Export] public string Text;
    [Export] public string HoverText;
    [Export] public EffectResource[] Effects;

    #endregion
}
