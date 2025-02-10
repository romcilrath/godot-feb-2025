using System;

public class Stat
{
    public string Name { get; protected set; }
    public float Min { get; protected set; }
    public float Max { get; protected set; }
    public float Current { get; protected set; }

    public Stat(string name, float min = float.MinValue, float max = float.MaxValue, float initial = 0)
    {
        Name = name;
        Min = min;
        Max = max;
        Current = Math.Clamp(initial, min, max);
    }

    public void Modify(float amount)
    {
        Current = Math.Clamp(Current + amount, Min, Max);
    }
}