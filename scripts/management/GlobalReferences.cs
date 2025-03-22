using Godot;
using System;
using System.Collections.Generic;

public partial class GlobalReferences : Node
{
    // Used to store single game instance
    // Accessed from other scripts like:
    //      GameManager.Instance.ChoiceInstanceScene;
    private static GlobalReferences _instance;
    public static GlobalReferences Instance => _instance;

    [Export] public PackedScene CardInstanceScene;
    [Export] public PackedScene ChoiceInstanceScene;
    [Export] public PackedScene IconInstanceScene;

    [Export] public Texture2D VitalityIcon;
    [Export] public Texture2D CoinIcon;
    [Export] public Texture2D RationsIcon;
    [Export] public Texture2D GritIcon;

    [Export] public Color VitalityColor;
    [Export] public Color CoinColor;
    [Export] public Color RationsColor;
    [Export] public Color GritColor;

    public override void _Ready() 
    {
        // Enforce singleton design pattern
        if (_instance != null)
        {
            GD.PrintErr("Multiple GlobalReferences instances detected! Deleting duplicate.");
            QueueFree();
            return;
        }

        _instance = this;
        GD.Print("GlobalReferences Initialized.");
    }
}