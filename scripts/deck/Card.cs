using System;
using System.Collections.Generic;
using Godot;

public class Card 
{
    public int Number { get; private set; }
    public string Name { get; private set; } = "Card Name";
    public string Body { get; private set; } = "Card Body";
    public Texture2D Art { get; private set; }
    public Effect[] CardEffects { get; private set; }
    public Choice[] Choices { get; private set; }
    
    public Card(int number = 0, string name = null, string body = null, Texture2D art = null, Choice[] choices = null, Effect[] cardEffects = null)
    {
        Number = number;
        if (Name is not null)
            Name = name;
        if (Body is not null)
            Body = body;
        if (Art is not null)
            Art = art;
        if (CardEffects is not null)
            CardEffects = cardEffects;
        if (Choices is not null)
            Choices = choices;
    }

    public Card(CardResource cardResource)
    {
        int number = cardResource.Number;
        string name = cardResource.Name;
        string body = cardResource.Body;
        Texture2D art = cardResource.Art;
        ChoiceResource[] choiceResources = cardResource.Choices;
        EffectResource[] cardEffectResources = cardResource.CardEffects;

        // Convert array of choiceResources to array of choices
        Choice[] choices = new Choice[choiceResources.Length];
        for (int i = 0; i < choiceResources.Length; i++)
        {
            if (choiceResources[i] == null) continue;
            choices[i] = new Choice(choiceResources[i]);
        }

        // Determine the specific type of each Effect based on the EffectResource)
        Effect[] cardEffects = EffectFactory.CreateEffects(cardEffectResources);
        
        Number = number;
        Name = name;
        Body = body;
        Art = art;
        Choices = choices;
        CardEffects = cardEffects;
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

    public void ApplyEffects()
    {
        foreach (Effect effect in CardEffects)
            effect.Apply();
    }   

    public void PrintCard()
    {
        GD.Print($"Number: {Number}");
        GD.Print($"Name: {Name}");
        GD.Print($"Body: {Body}");
        GD.Print($"Art: {Art}");
        GD.Print($"Effects: {CardEffects.Length}");
        for (int i = 0; i < CardEffects.Length; i++)
        {
            GD.Print($"Card Effect #{i+1}:");
            CardEffects[i].PrintEffect();
        }
        GD.Print($"Choices: {Choices.Length}");
        for (int i = 0; i < Choices.Length; i++)
        {
            GD.Print($"Choice #{i+1}:");
            Choices[i].PrintChoice();
        }
    }
}