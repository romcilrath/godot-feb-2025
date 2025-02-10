using System;
using Godot;

public class RepeatEffect: Effect
{
    public int EveryTurn { get; private set; }
    public int ForTurns { get; private set; }
    private int CurrentTurn { get; set; }

    public RepeatEffect(Stat target, float value, ActionType actionType = ActionType.Add, int everyTurn = 1, int forTurns = 2)
        :base(target, value, actionType)
    {
        EveryTurn = everyTurn;
        ForTurns = forTurns;
    }

    public override void Apply()
    {
        RepeatApply();
    }

    private void ApplyOnce()
    {
        switch (ActionType)
        {
            case ActionType.Set:
                Target.Set(Value);
                break;
            case ActionType.Add:
                Target.Add(Value);
                break;
            case ActionType.Multiply:
                Target.Multiply(Value);
                break;
            default:
                GD.Print($"Cannot apply OneTimeEfect with ActionType: {ActionType}");
                break;
        }
    }

    private void RepeatApply()
    {
        // Check if the effect has not expired...
        if (CurrentTurn < ForTurns)
        {
            // Increment the current turn counter
            CurrentTurn += 1;

            // Unsubscribe to prevent memory leaks
            GameManager.Instance.OnTurnIncremented -= ApplyOnce;
            GameManager.Instance.OnTurnIncremented -= RepeatApply;

            // Subscribe 
            GameManager.Instance.OnTurnIncremented += ApplyOnce;
            GameManager.Instance.OnTurnIncremented += RepeatApply;
        }
        // If it has expired...
        else
        {
            // Unsubscribe to prevent memory leaks
            GameManager.Instance.OnTurnIncremented -= ApplyOnce;
            GameManager.Instance.OnTurnIncremented -= RepeatApply;

            GD.Print("RepeatEffect expired.");
        }
    }
}