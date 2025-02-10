using System.Collections.Generic;
using Godot;

public class Card 
{
    public string Name { get; private set; } = "Card Name";
    public string Body { get; private set; } = "Card Body";
    public Sprite2D Art { get; private set; }
    public List<Choice> Choices { get; private set; } = new List<Choice>();

    public Card(string name = null, string body = null, Sprite2D art = null, List<Choice> choices = null)
    {
        if (Name is not null)
            Name = name;
        if (Body is not null)
            Body = body;
        if (Art is not null)
            Art = art;
        if (Choices is not null)
            Choices = choices;
    }
}