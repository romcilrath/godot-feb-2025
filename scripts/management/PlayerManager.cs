using Godot;
using System;
using System.Collections.Generic;

public enum Backstory
{
    Adventurer
}

public enum Race
{
    Human
}

public partial class PlayerManager : Node
{
    // Used to store single game instance
    // Accessed from other scripts like:
    //      PlayerManager.Instance.AddScore(10);
    private static PlayerManager _instance;
    public static PlayerManager Instance => _instance;

    // Player characteristics
    public string PlayerName { get; private set; } = "Player Name";
    public int Age { get; private set; } = 12;
    public Race Race { get; private set; } = Race.Human;
    public Backstory Backstory { get; private set; } = Backstory.Adventurer;

    // Player stats
    public Stat Money { get; private set; } = new Stat("Money");                        // Money
    public Stat Health { get; private set; } = new Stat("Health", min:100, max:100);    // Health
    public Stat Armor { get; private set; } = new Stat("Armor");                        // Armor
    public Stat Attack { get; private set; } = new Stat("Attack", initial:1f);          // Attack

    public override void _Ready() 
    {
        // Enforce singleton design pattern
        if (_instance != null)
        {
            GD.PrintErr("Multiple PlayerManager instances detected! Deleting duplicate.");
            QueueFree();
            return;
        }

        _instance = this;
        GD.Print("PlayerManager Initialized.");

        Debug_Apply_Choice();
        Debug_Load_Card();
    }

    public void PrintStats()
    {
        GD.Print($"Money: {Money.Current}");
        GD.Print($"Health: {Health.Current}");
        GD.Print($"Armor: {Armor.Current}");
        GD.Print($"Attack: {Attack.Current}");
    }

    public void Debug_Load_Card()
    {
        GD.Print("GameManager debug");        
        CardResource cardResource = ResourceLoader.Load<CardResource>("res://resources/cards/test_card.tres");
        Card card = new Card(cardResource);
        card.PrintCard();
    }

    public void Debug_Apply_Choice()
    {
        Effect test1 = new OneTimeEffect(Money, 10f, ActionType.Set);

        Effect test2 = new RepeatEffect(Money, 10f, ActionType.Multiply, 1, 5);

        Effect[] effects = { test1, test2 };
        Choice choice = new Choice(effects:effects);
        choice.Apply();

        GD.Print(GameManager.Instance.Turn);
        PrintStats();

        for (int i = 0; i < 7; i++)
        {
            GameManager.Instance.IncrementTurn();
            GD.Print(GameManager.Instance.Turn);
            PrintStats();
        }
    }
}
