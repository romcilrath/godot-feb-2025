using System;
using System.Collections.Generic;
using Godot;

public class Deck
{
    public string Name { get; private set; } = "Deck Name";
    public Texture2D Art { get; private set; }
    public List<Card> Cards { get; private set; }
    public List<Card> LockedCards { get; private set; }
    public List<Card> DiscardedCards { get; private set; }

    public Deck(string name = null, Texture2D Art = null, List<Card> cards = null, List<Card> lockedCards = null)
    {
        if (name is not null)
            Name = name;
        Cards = cards ?? new List<Card>();
        LockedCards = lockedCards ?? new List<Card>();
        DiscardedCards = new List<Card>();
    }

    public Deck(DeckResource deckResource)
    {
        string name = deckResource.Name;
        Texture2D art = deckResource.Art;
        CardResource[] cardResources = deckResource.Cards;
        CardResource[] lockedCardResources = deckResource.LockedCards;
        

        // Convert  array of cardResources to array of cards
        Card[] cardsArray = new Card[cardResources.Length];
        for (int i = 0; i < cardResources.Length; i++)
        {
            cardsArray[i] = new Card(cardResources[i]);
        }

        // Convert  array of lockedCardResources to array of cards
        Card[] lockedCardsArray = new Card[lockedCardResources.Length];
        for (int i = 0; i < lockedCardResources.Length; i++)
        {
            lockedCardsArray[i] = new Card(lockedCardResources[i]);
        }

        Name = name;
        Art = art;
        Cards = new List<Card>(cardsArray);
        DiscardedCards = new List<Card>(lockedCardsArray);
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
