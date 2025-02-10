using Godot;
using System;

public partial class GameManager : Node
{
    // Used to store single game instance
    // Accessed from other scripts like:
    //      GameManager.Instance.AddScore(10);
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    // Global game states
    public int Turn { get; private set; } = 0;

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
        GD.Print($"Incremented Turn from {Turn} to {Turn + 1}");
        Turn += incrementBy;
    }
}
