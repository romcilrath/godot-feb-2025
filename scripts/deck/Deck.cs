using System;
using System.Collections.Generic;
using Godot;

public class Deck
{
    public string Name { get; private set; } = "Deck Name";
    public List<Card> Cards { get; private set; }
    public List<Card> DiscardedCards { get; private set; }

    public Deck(string name = null, List<Card> cards = null)
    {
        if (name is not null)
            Name = name;
        Cards = cards ?? new List<Card>();
        DiscardedCards = new List<Card>();
    }

    public Card Draw()
    {
        if (Cards.Count == 0)
        {
            Refresh();
            if (Cards.Count == 0) // If still empty after refresh, return null
                return null;
        }
        
        int drawIndex = GD.RandRange(0, Cards.Count - 1);
        Card card = Cards[drawIndex];
        Cards.RemoveAt(drawIndex);
        DiscardedCards.Add(card);
        return card;
    }

    public void Shuffle()
    {
        Random rng = new Random();
        int n = Cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (Cards[k], Cards[n]) = (Cards[n], Cards[k]);
        }
    }

    public void Refresh()
    {
        if (DiscardedCards.Count == 0)
            return;

        Cards.AddRange(DiscardedCards);
        DiscardedCards.Clear();
        Shuffle();
    }
}
