using System;
using System.Collections.Generic;
using Godot;

public class Card 
{
    public string Name { get; private set; } = "Card Name";
    public string Body { get; private set; } = "Card Body";
    public Texture2D Art { get; private set; }
    public Choice[] Choices { get; private set; }
    
    public Card(string name = null, string body = null, Texture2D art = null, Choice[] choices = null)
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

    public Card(CardResource cardResource)
    {
        string name = cardResource.Name;
        string body = cardResource.Body;
        Texture2D art = cardResource.Art;
        ChoiceResource[] choiceResources = cardResource.Choices;

        // Convert  array of choiceResources to array of choices
        Choice[] choices = new Choice[choiceResources.Length];
        for (int i = 0; i < choiceResources.Length; i++)
        {
            choices[i] = new Choice(choiceResources[i]);
        }
        
        Name = name;
        Body = body;
        Art = art;
        Choices = choices;
    }

    public Choice SelectChoice(int choiceIndex = 0)
    {
        if (Choices == null || Choices.Length == 0)
            return null;
        if (choiceIndex < 0 || choiceIndex >= Choices.Length)
            throw new ArgumentOutOfRangeException(nameof(choiceIndex), "Choice index is out of range.");
        
        Choices[choiceIndex].Apply();
        
        return Choices[choiceIndex];
    }

    public void PrintCard()
    {
        GD.Print($"Name: {Name}");
        GD.Print($"Body: {Body}");
        GD.Print($"Art: {Art}");
        GD.Print($"Choices: {Choices.Length}");
        for (int i = 0; i < Choices.Length; i++)
        {
            GD.Print($"Choice #{i+1}:");
            Choices[i].PrintChoice();
        }
    }
}