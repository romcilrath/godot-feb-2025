using Godot;
using System;

[GlobalClass]
public partial class CardResource : Resource
{
    #region VARIABLES

    [Export] public string Name;
    [Export(PropertyHint.MultilineText)] public string Body;
    [Export] public Texture2D Art;
    [Export] public ChoiceResource[] Choices;

    #endregion
}
