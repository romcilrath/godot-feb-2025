using System;
using Godot;

public class OneTimeStatEffect: StatEffect
{
    public OneTimeStatEffect(Stat target, float value, ActionType actionType = ActionType.Add)
        :base(target, value, actionType)
    {

    }

    public OneTimeStatEffect(OneTimeStatEffectResource oneTimeEffectResource)
        :base(oneTimeEffectResource)
    { 

    }

    public override void Apply()
    {
        PlayerStatRenderer targetRenderer = null;
        switch(Target.Name)
        {
            case "Coin":
                targetRenderer = GameManager.Instance.CoinRenderer;
                break;
            case "Vitality":
                targetRenderer = GameManager.Instance.VitalityRenderer;
                break;
            case "Grit":
                targetRenderer = GameManager.Instance.GritRenderer;
                break;
            case "Rations":
                targetRenderer = GameManager.Instance.RationsRenderer;
                break;
            default:
                GD.Print($"Cannot tarrget StatRender with Target named: {Target.Name}");
                break;
        }

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
}