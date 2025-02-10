using Godot;
using System;

public partial class GameManager : Node
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

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
        GD.Print("GameManager Initialized");
    }
}
