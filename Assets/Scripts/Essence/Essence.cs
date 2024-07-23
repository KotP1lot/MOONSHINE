using UnityEngine;

public class Essence : ScriptableObject
{
    public enum EssenceType
    {
        Alcohol,
        Toxicity,
        Sweetness,
        Bitterness,
        Sourness
    }

    public EssenceType Type;
    public float Strength;

    public string GetStrengthLevel()
    {
        if (Strength <= 25) return "Weak";
        if (Strength <= 50) return "Moderate";
        if (Strength <= 75) return "Strong";
        return "Epic";
    }
}
