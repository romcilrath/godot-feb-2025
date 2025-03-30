using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Vector2 = Godot.Vector2;

public partial class GameManager : Node
{
	// Used to store single game instance
	// Accessed from other scripts like:
	//      GameManager.Instance.AddScore(10);
	private static GameManager _instance;
	public static GameManager Instance => _instance;

	[Export] public DeckResource[] StartingDecks { get; private set; }
	[Export] public Node2D CardSpawnPoint { get; private set; }
	[Export] public Node2D CardExitPoint { get; private set; }
	[Export] public ReferenceRect CardDrawRegion { get; private set; }

	// Define an event that gets triggered when Turn increments
	public event Action OnTurnIncremented;

	// Define Turn
	public int Turn { get; private set; } = 1;  // Start Turn at 1

	// Define ActiveDecks
	public List<Deck> ActiveDecks { get; private set; } = new List<Deck>();

	public override void _Ready() 
	{
		// Enforce singleton design pattern
		if (_instance != null)
		{
			GD.PrintErr("Multiple GameManager instances detected! Deleting duplicate.");
			QueueFree();
			return;
		}

		// This sets upa  call to _LateReady right after _Ready for activities that require other _Ready's to have completed
		// Instantiating the Deck for example requires PlayManager to reference the Stats
		CallDeferred(nameof(_AfterReady));
	
		_instance = this;
		GD.Print("GameManager Initialized.");
	}
	
	// Called directly after _Ready
	private void _AfterReady()
	{
		// Add starting Decks after PlayerManager instanced
		AddStartingDecks();

		GD.Print(GetActiveCardCount());
		OnDrawFromActiveDecks(3);
		GD.Print(GetActiveCardCount());
	}

	public void AddStartingDecks()
	{
		GD.Print("Adding StartingDecks to ActiveDecks...");
		foreach (DeckResource deckResource in StartingDecks)
		{
			Deck deck = new Deck(deckResource);
			ActiveDecks.Add(deck);
		}
	}

	public void IncrementTurn(int incrementBy = 1)
	{
		// Increment the Turn
		float oldTurn = Turn;
		Turn += incrementBy;
		GD.Print($"Incremented Turn from {oldTurn} to {Turn}");

		// Notify the OnTurnIncremented listeners
		OnTurnIncremented?.Invoke();
	}

	public int GetActiveCardCount()
	{
		// List the sum of the Cards in all ActiveDecks
		int activeCardCount = 0;
		for (int index = 0; index < ActiveDecks.Count; index++)
		{
			activeCardCount += ActiveDecks[index].Cards.Count;
		}
		return activeCardCount;
	}

	public Card[] DrawFromActiveDecks(int count = 1)
	{
		// Draw a random Card from ActiveDecks
		Card[] cards = new Card[count];

		for (int j = 0; j < count; j++)
		{
			int cardCount = GetActiveCardCount();

			if (cardCount == 0) return cards;

			int chosenCardIndex = (int)(GD.Randi() % cardCount);

			int choosenDeckIndex = 0;
			int deckCardIndex = 0;
			for (int index = 0; index < chosenCardIndex; index++)
			{
				if (deckCardIndex > ActiveDecks[choosenDeckIndex].Cards.Count)
				{
					choosenDeckIndex += 1;
					deckCardIndex = 0;
				}
				else
				{
					deckCardIndex += 1;
				}
			}
			Card newCard = ActiveDecks[choosenDeckIndex].DrawAt(deckCardIndex);
			cards[j] = newCard;
		}
		return cards;
	}

	private void OnDrawFromActiveDecks(int count = 1)
	{
		Card[] cards = DrawFromActiveDecks(count);
		GD.Print("Cards: " + cards.Length);

		for (int i = 0; i < cards.Length; i++)
		{
			Card card = cards[i];
			CardFlipper cardFlipper = GlobalReferences.Instance.CardFlipperScene.Instantiate() as CardFlipper;
			GetTree().Root.AddChild(cardFlipper);
			cardFlipper.Position = CardSpawnPoint.Position;
			cardFlipper.Scale = new Vector2(0.35f, 0.35f);

			CardInstance cardInstance = cardFlipper.GetCardInstance();

			cardInstance.SetCard(card);
			cardInstance.LoadCard();
			cardInstance.FlipCard();
			
			cardFlipper.HideEffects();

			Effect[] effects = cardInstance.GetCard().CardEffects;
			cardFlipper.LoadCardEffects(effects);
			
			float xPosition = CardDrawRegion.Position.X + ((i + 0.5f) * (CardDrawRegion.Size.X / cards.Length));
			float yPosition = CardDrawRegion.Position.Y + CardDrawRegion.Size.Y/2;
			Vector2 toPosition = new Vector2(xPosition, yPosition);

			Tween tween = CreateTween();
			tween.TweenProperty(cardFlipper, "position", toPosition, 0.8)
				.SetEase(Tween.EaseType.Out)
				.SetTrans(Tween.TransitionType.Elastic);
			
		}
	}
}


