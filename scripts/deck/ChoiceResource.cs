using Godot;

[GlobalClass]
public partial class ChoiceResource : Resource
{
    #region VARIABLES

    [Export(PropertyHint.MultilineText)] public string Text;
    [Export(PropertyHint.MultilineText)] public string HoverText;
    [Export] public EffectResource[] Effects;

    #endregion
}
