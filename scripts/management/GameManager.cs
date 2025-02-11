using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
    // Used to store single game instance
    // Accessed from other scripts like:
    //      GameManager.Instance.AddScore(10);
    private static GameManager _instance;
    public static GameManager Instance => _instance;

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

        _instance = this;
        GD.Print("GameManager Initialized.");
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

    public void Debug()
    {
        GD.Print("GameManager debug");        
        CardResource cardResource = ResourceLoader.Load<CardResource>("res://resources/test_card.tres");
        Card card = new Card(cardResource);
        GD.Print(card.Body);
        card.PrintCard();
    }
}
