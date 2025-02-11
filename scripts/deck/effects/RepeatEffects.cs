using System;
using Godot;

public class RepeatEffect : Effect
{
    public int EveryTurn { get; private set; } = 1;
    public int ForTurns { get; private set; }
    private int CurrentTurn { get; set; } = 0;

    // Constructor using explicit parameters
    public RepeatEffect(Stat target, float value, ActionType actionType = ActionType.Add, int everyTurn = 1, int forTurns = 2)
        : base(target, value, actionType)
    {
        EveryTurn = everyTurn;
        ForTurns = forTurns;
    }

    // Constructor using RepeatEffectResource
    public RepeatEffect(RepeatEffectResource repeatEffectResource) 
        : base(repeatEffectResource)
    {
        EveryTurn = repeatEffectResource.EveryTurn;
        ForTurns = repeatEffectResource.ForTurns;
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
                GD.Print($"Cannot apply RepeatEffect with ActionType: {ActionType}");
                break;
        }
    }

    private void RepeatApply()
    {
        if (CurrentTurn < ForTurns)
        {
            CurrentTurn++;

            // Unsubscribe to avoid memory leaks
            GameManager.Instance.OnTurnIncremented -= ApplyOnce;
            GameManager.Instance.OnTurnIncremented -= RepeatApply;

            // Subscribe for future turns
            GameManager.Instance.OnTurnIncremented += ApplyOnce;
            GameManager.Instance.OnTurnIncremented += RepeatApply;
        }
        else
        {
            // Unsubscribe when effect expires
            GameManager.Instance.OnTurnIncremented -= ApplyOnce;
            GameManager.Instance.OnTurnIncremented -= RepeatApply;

            GD.Print("RepeatEffect expired.");
        }
    }
}
