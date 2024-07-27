using UnityEngine;

public enum StatType
{
    Alcohol,
    Toxicity,
    Sweetness,
    Bitterness,
    Sourness
}
public class Essence : ScriptableObject
{
    public StatType Type;
    public float Strength;

    public string GetStrengthLevel()
    {
        if (Strength <= 25) return "Weak";
        if (Strength <= 50) return "Moderate";
        if (Strength <= 75) return "Strong";
        return "Epic";
    }
}
