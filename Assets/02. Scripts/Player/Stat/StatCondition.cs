using UnityEngine;

[System.Serializable]
public class StatCondition
{
    public float currentValue;
    public float maxValue;

    public StatCondition(float max)
    {
        maxValue = max;
        currentValue = max;
    }

    public void AddStat(float value)
    {
        currentValue = Mathf.Min(currentValue + value, maxValue);
    }

    public void DecreaseStat(float value)
    {
        currentValue = Mathf.Max(currentValue - value, 0f);
    }
}
