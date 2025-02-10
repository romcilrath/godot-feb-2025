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
    public Stat Money { get; private set; } = new Stat("Money");
    public Stat Health { get; private set; } = new Stat("Health", 0, 100, 100);
    public Stat Armor { get; private set; } = new Stat("Armor");
    public Stat Attack { get; private set; } = new Stat("Attack", 0, float.MaxValue, 1f);

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

        PrintStats();
    }

    public void PrintStats()
    {
        GD.Print($"Money: {Money.Current}");
        GD.Print($"Health: {Health.Current}");
        GD.Print($"Armor: {Armor.Current}");
        GD.Print($"Attack: {Attack.Current}");
    }
}
