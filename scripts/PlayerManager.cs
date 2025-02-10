using Godot;
using System;

public partial class PlayerManager : Node
{
    // Used to store single game instance
    // Accessed from other scripts like:
    //      PlayerManager.Instance.AddScore(10);
    private static PlayerManager _instance;
    public static PlayerManager Instance => _instance;

    // Global game states
    public Stat Money { get; private set; } = new Stat("Money");                            // Money
    public Stat Health { get; private set; } = new Stat("Health", min:100, max:100);             // Health
    public Stat Armor { get; private set; } = new Stat("Armor");                            // Armor
    public Stat Attack { get; private set; } = new Stat("Attack", initial:1f);   // Attack

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

        Effect test2 = new OneTimeEffect(Money, 10f, ActionType.Set);
        test2.Apply();

        Effect test = new RepeatEffect(Money, 10f, ActionType.Multiply, 1, 5);
        test.Apply();

        GD.Print(GameManager.Instance.Turn);
        PrintStats();

        for (int i = 0; i < 7; i++)
        {
            GameManager.Instance.IncrementTurn();
            GD.Print(GameManager.Instance.Turn);
            PrintStats();
        }
    }

    public void PrintStats()
    {
        GD.Print($"Money: {Money.Current}");
        GD.Print($"Health: {Health.Current}");
        GD.Print($"Armor: {Armor.Current}");
        GD.Print($"Attack: {Attack.Current}");
    }
}
