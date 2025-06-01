using System;
using Godot;

public class RepeatStatEffect : StatEffect
{
    public int EveryTurn { get; private set; } = 1;
    public int ForTurns { get; private set; }
    private int CurrentTurn { get; set; } = 0;

    // Constructor using explicit parameters
    public RepeatStatEffect(Stat target, float value, ActionType actionType = ActionType.Add, int everyTurn = 1, int forTurns = 2)
        : base(target, value, actionType)
    {
        EveryTurn = everyTurn;
        ForTurns = forTurns;
    }

    // Constructor using RepeatEffectResource
    public RepeatStatEffect(RepeatStatEffectResource repeatEffectResource) 
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
        PlayerStatRenderer targetRenderer = GetTargetRenderer();

        Color targetRenderColor = Colors.Black;
        if (ActionType == ActionType.Set)
            targetRenderColor = Colors.Yellow;

        switch (ActionType)
        {
            case ActionType.Set:
                Target.Set(Value);
                if (Value < Target.Current)
                    targetRenderColor = Colors.Red;
                else if (Value > Target.Current)
                    targetRenderColor = Colors.Green;
                break;
            case ActionType.Add:
                Target.Add(Value);
                if (Value < 0)
                    targetRenderColor = Colors.Red;
                else if (Value > 0)
                    targetRenderColor = Colors.Green;
                break;
            case ActionType.Multiply:
                Target.Multiply(Value);
                if (Value < 1)
                    targetRenderColor = Colors.Red;
                else if (Value > 1)
                    targetRenderColor = Colors.Green;
                break;
            default:
                GD.Print($"Cannot apply OneTimeEfect with ActionType: {ActionType}");
                break;
        }
        targetRenderer.Shake(targetRenderColor);
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

    public override void PrintEffect()
    {
        base.PrintEffect();
        GD.Print($"Every Turn: {EveryTurn}");
        GD.Print($"For Turns: {ForTurns}");
        GD.Print($"Current Turn: {CurrentTurn}");
    }
}
