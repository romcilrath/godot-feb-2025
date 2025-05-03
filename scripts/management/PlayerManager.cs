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
    private const int MAX_STAT_VALUE = 9999;   // Constants should use 'const' and be in uppercase

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
        Coin = new Stat("Coin", min:0, max:MAX_STAT_VALUE, initial:StartingCoin);
        Vitality = new Stat("Vitality", min:0, max:100, initial:StartingVitality);
        Grit = new Stat("Grit", min:0, max:MAX_STAT_VALUE, initial:StartingGrit);
        Rations = new Stat("Rations", min:0, max:MAX_STAT_VALUE, initial:StartingRations);
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
}
