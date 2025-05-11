using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Vector2 = Godot.Vector2;

public partial class GameManager : Node
{
	// Used to store single game instance
	// Accessed from other scripts like:
	//      GameManager.Instance.AddScore(10);
	private static GameManager _instance;
	public static GameManager Instance => _instance;

	[Export] public DeckResource[] StartingDecks { get; private set; }
	[Export] public PlayerStatRenderer CoinRenderer { get; private set; }
	[Export] public PlayerStatRenderer VitalityRenderer { get; private set; }
	[Export] public PlayerStatRenderer GritRenderer { get; private set; }
	[Export] public PlayerStatRenderer RationsRenderer { get; private set; }
	[Export] public Node2D CardSpawnPoint { get; private set; }
	[Export] public Node2D CardExitPoint { get; private set; }
	[Export] public ReferenceRect CardDrawRegion { get; private set; }
	[Export] public Node2D ShufflingIndicator { get; private set; }

	// Define an event that gets triggered when Turn increments
	public event Action OnTurnIncremented;

	// Define Turn
	public int Turn { get; private set; } = 1;  // Start Turn at 1

	// Define ActiveDecks
	public List<Deck> ActiveDecks { get; private set; } = new List<Deck>();
	public CardFlipper[] BlindDraw { get; private set; }

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

		DoBlindDraw(3);
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

	public async void DelayIncrementTurn(int incrementBy = 1, int delay = 1000)
	{
        await Task.Delay(delay); 
		IncrementTurn(incrementBy);
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
		int cardCount = GetActiveCardCount();
		int _toDrawCount = count > cardCount ? cardCount : count;
		Card[] cards = new Card[_toDrawCount];

		for (int i = 0; i < _toDrawCount; i++)
		{
			if (cardCount == 0) return cards;

			int chosenCardIndex = (int)(GD.Randi() % cardCount);
			int toDrawDeckIndex = 0;
			int toDrawCardIndex = chosenCardIndex;

			while (true)
			{
				int deckSize = ActiveDecks[toDrawDeckIndex].Cards.Count;
				if (toDrawCardIndex < deckSize)
					break;

				toDrawCardIndex -= deckSize;
				toDrawDeckIndex += 1;
			}

			Card newCard = ActiveDecks[toDrawDeckIndex].BlindDrawAt(toDrawCardIndex);
			cards[i] = newCard;

			cardCount -= 1;
		}

		return cards;
	}


	private void DoBlindDraw(int count = 1)
	{
		Card[] cards = DrawFromActiveDecks(count);
		BlindDraw = new CardFlipper[cards.Length];
		GD.Print("Cards: " + cards.Length);

		for (int i = 0; i < cards.Length; i++)
		{
			Card card = cards[i];
			CardFlipper cardFlipper = GlobalReferences.Instance.CardFlipperScene.Instantiate() as CardFlipper;
			GetTree().Root.AddChild(cardFlipper);
			BlindDraw[i] = cardFlipper;
			cardFlipper.Position = CardSpawnPoint.Position;
			cardFlipper.Scale = new Vector2(0.3f, 0.3f);

			CardInstance cardInstance = cardFlipper.GetCardInstance();
			
			// Hook up signals for when card back is selected, for emiting a card selected signal
			// And when a card choice is selected, for emitting a card dismissed signal
			cardInstance.Connect(CardInstance.SignalName.OnCardSelected, Callable.From((CardInstance selectedCardInstance) => OnCardSelected(selectedCardInstance)));
			cardInstance.Connect(CardInstance.SignalName.OnCardDismissed, Callable.From((CardInstance cardInstance) => OnCardDismissed(cardInstance)));

			// Assign and then flipe the card
			cardInstance.SetCard(card);
			cardInstance.LoadCard();
			cardInstance.FlipCard();

			Effect[] effects = cardInstance.GetCard().CardEffects;
			cardFlipper.LoadCardEffects(effects);
			cardFlipper.HideEffects();
			
			float xPosition = CardDrawRegion.Position.X + ((i + 0.5f) * (CardDrawRegion.Size.X / cards.Length));
			float yPosition = CardDrawRegion.Position.Y + CardDrawRegion.Size.Y/2;
			Vector2 toPosition = new Vector2(xPosition, yPosition);

			Tween tween = CreateTween();
			tween.TweenProperty(cardFlipper, "position", toPosition, 0.8)
				.SetEase(Tween.EaseType.Out)
				.SetTrans(Tween.TransitionType.Elastic);
		}
	}

	private void OnCardSelected(CardInstance selectedCardInstance)
	{
		foreach (CardFlipper cardFlipper in BlindDraw)
		{
			CardInstance cardInstance = cardFlipper.GetCardInstance();

			// Dismiss unselected cards
			if (cardInstance != selectedCardInstance)
			{
				cardInstance.SetEnabled(false);

				Vector2 toPosition = new Vector2(cardFlipper.Position.X, CardExitPoint.Position.Y);
				Tween positionTween = CreateTween();
				positionTween.TweenProperty(cardFlipper, "position", toPosition, 0.4f)
					.SetDelay(0.5)
					.SetEase(Tween.EaseType.In);
				positionTween.Finished += () => cardFlipper.QueueFree();
			}
			// Flip, scale, and move to center selected card 
			else
			{				
				cardFlipper.ZIndex += 1;
				
				cardFlipper.OnScaleCard(0.4f, 0.4f, 1f);

				Vector2 toPosition = CardDrawRegion.Position + CardDrawRegion.Size/2;
				Tween positionTween = CreateTween();
				positionTween.TweenProperty(cardFlipper, "position", toPosition, 0.4f)
					.SetDelay(0.5)
					.SetEase(Tween.EaseType.In);

				positionTween.Finished += () => cardFlipper.OnFlipRight();
				positionTween.Finished += () => cardFlipper.OnAnimateApplyCardEffects(1f);
			}
		}
	}

	private void OnCardDismissed(CardInstance cardInstance)
	{
		Card[] toDiscard = new Card[1];
		toDiscard[0] = cardInstance.GetCard();
		foreach (Deck deck in ActiveDecks)
		{
			deck.ReturnBlindDrawnCards(toDiscard);
		}
		// Vertically transition card to CardExitPoints Y position
		foreach (CardFlipper cardFlipper in BlindDraw)
		{
			// Skip if its not the card to be dismissed
			if (cardFlipper.GetCardInstance() != cardInstance) continue;

			Vector2 toPosition = new Vector2(cardFlipper.Position.X, CardExitPoint.Position.Y);
			Tween positionTween = CreateTween();
			positionTween.TweenProperty(cardFlipper, "position", toPosition, 0.4f)
				.SetDelay(0.5)
				.SetEase(Tween.EaseType.In);
			positionTween.Finished += () => HandleTurn();
			positionTween.Finished += () => cardFlipper.QueueFree();
		}
	}

	private void HandleTurn()
	{
		List<Deck> toRefreshDecks = new List<Deck>();
		foreach (Deck deck in ActiveDecks)
		{
			if (deck.IsExhausted()) toRefreshDecks.Add(deck);
		}

		foreach (Deck deck in toRefreshDecks)
		{
			GD.Print($"Refreshing deck {deck.Name}...");
			deck.Refresh();
		}

		if (toRefreshDecks.Count > 0)
		{
			ShufflingIndicator.Visible = true;
			ShufflingIndicator.Modulate = new Color(ShufflingIndicator.Modulate, 0);

			Tween showShuffleIndicator = CreateTween();
			showShuffleIndicator.TweenProperty(ShufflingIndicator, "modulate:a", 1, 0.1f)
				.SetEase(Tween.EaseType.In);
			showShuffleIndicator.TweenInterval(3);
			showShuffleIndicator.TweenProperty(ShufflingIndicator, "modulate:a", 0, 0.1f)
				.SetEase(Tween.EaseType.Out);

			showShuffleIndicator.Finished += () => ShufflingIndicator.Visible = false;
			showShuffleIndicator.Finished += () => DoBlindDraw(3);
		} else {
			DoBlindDraw(3);
		}
	}
}


