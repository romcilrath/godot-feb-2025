using Godot;
using System;

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
}
