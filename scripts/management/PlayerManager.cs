using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerManager : Node
{
    // Used to store single game instance
    // Accessed from other scripts like:
    //      PlayerManager.Instance.AddScore(10);
    private static PlayerManager _instance;
    public static PlayerManager Instance => _instance;

    [Export] public int StartingCoin = 25;
    [Export] public int StartingVitality = 100;
    [Export] public int StartingGrit = 10;
    [Export] public int StartingRations = 20;

    // Player stats
    public Stat Coin { get; private set; }
    public Stat Vitality { get; private set; } 
    public Stat Grit { get; private set; }
    public Stat Rations { get; private set; }

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

        // Initialize Vitality with StartingVitality
        Coin = new Stat("Coin", min:0, initial:StartingCoin);
        Vitality = new Stat("Vitality", min:0, max:100, initial:StartingVitality);
        Grit = new Stat("Grit", min:0, initial:StartingGrit);
        Rations = new Stat("Rations", initial:StartingRations);

        //Debug_Apply_Choice();
    }

    public List<Stat> GetStats()
    {
        return new List<Stat> { Coin, Vitality, Grit, Rations };
    }

    public void PrintStats()
    {
        GD.Print($"Coin: {Coin.Current}");
        GD.Print($"Vitality: {Vitality.Current}");
        GD.Print($"Grit: {Grit.Current}");
        GD.Print($"Rations: {Rations.Current}");
    }

    public void Debug_Apply_Choice()
    {
        Effect test1 = new OneStatTimeEffect(Coin, 10f, ActionType.Set);

        Effect test2 = new RepeatStatEffect(Coin, 10f, ActionType.Multiply, 1, 5);

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
