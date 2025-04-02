using System;
using System.Collections.Generic;
using Godot;

public class Deck
{
    public string Name { get; private set; } = "Deck Name";
    public Texture2D Art { get; private set; }
    public List<Card> Cards { get; private set; }
    public List<Card> BlindDrawnCards { get; private set; }
    public List<Card> LockedCards { get; private set; }
    public List<Card> DiscardedCards { get; private set; }

    public Deck(string name = null, Texture2D art = null, List<Card> cards = null, List<Card> blindDrawnCards = null, List<Card> lockedCards = null)
    {
        Name = name ?? "Deck Name";
        Art = art;  // TODO: Handle null art
        Cards = cards ?? new List<Card>();
        BlindDrawnCards = blindDrawnCards ?? new List<Card>();
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
        BlindDrawnCards = new List<Card>();
        LockedCards = new List<Card>(lockedCardsArray);
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
        return DrawAt(drawIndex);
    }

    public Card DrawAt(int drawIndex = 0)
    {
        if (Cards.Count == 0 || drawIndex > Cards.Count)
        {
            Refresh();
            if (Cards.Count == 0)
                return null;
        }
        
        Card card = Cards[drawIndex];
        Cards.RemoveAt(drawIndex);
        DiscardedCards.Add(card);
        return card;
    }

    public Card BlindDrawAt(int drawIndex = 0)
    {
        if (Cards.Count == 0 || drawIndex > Cards.Count)
        {
            Refresh();
            if (Cards.Count == 0)
                return null;
        }
        
        Card card = Cards[drawIndex];
        Cards.RemoveAt(drawIndex);
        BlindDrawnCards.Add(card);
        return card;
    }

    public void ReturnBlindDrawnCards(Card[] toDiscard)
    {
        for (int i = 0; i < BlindDrawnCards.Count; i++)
        {
            Card returnCard = BlindDrawnCards[i];
            bool isToDiscard = false;
            foreach (Card discardCard in toDiscard) if (discardCard == returnCard) isToDiscard = true; 
            if (isToDiscard) DiscardedCards.Add(returnCard);
            else Cards.Add(returnCard);
        }
        BlindDrawnCards.Clear();
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

    public bool IsExhausted()
    {
        if (Cards.Count == 0 && DiscardedCards.Count > 0) return true;
        return false;
    }

    public void PrintDeck()
    {
        GD.Print($"Deck Name: {Name}");
        GD.Print($"Deck Art: {Art}");
        GD.Print($"Cards: {Cards.Count}");
        for (int i = 0; i < Cards.Count; i++)
        {
            GD.Print($"Choice #{i+1}:");
            Cards[i].PrintCard();
        }
        GD.Print($"Locked Cards: {LockedCards.Count}");
        for (int i = 0; i < LockedCards.Count; i++)
        {
            GD.Print($"Choice #{i+1}:");
            Cards[i].PrintCard();
        }
        GD.Print($"Discarded Cards: {DiscardedCards.Count}");
        for (int i = 0; i < DiscardedCards.Count; i++)
        {
            GD.Print($"Choice #{i+1}:");
            Cards[i].PrintCard();
        }
    }
}
