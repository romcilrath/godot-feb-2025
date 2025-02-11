using Godot;

[GlobalClass]
public partial class DeckResource : Resource
{
    #region VARIABLES

    [Export] public string Name;
    [Export] public Texture2D Art;
    [Export] public CardResource[] Cards;
    [Export] public CardResource[] LockedCards;

    #endregion
}
