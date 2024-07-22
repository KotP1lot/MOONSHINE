using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Centrifuge : MonoBehaviour
{
    public enum EssenceType
    {
        Alcohol,
        Toxicity,
        Sweetness,
        Bitterness,
        Sourness,
        Uniqueness
    }

    public enum EssenceQuality
    {
        Weak,
        Decent,
        Strong,
        Epic
    }

    [System.Serializable]
    public class Essence
    {
        public EssenceType Type;
        public EssenceQuality Quality;
    }

    [SerializeField] private List<Essence> _essences;

    public SOIngredient ProcessIngredient(SOIngredient ingredient)
    {
        if (ingredient == null)
        {
            Debug.Log("No ingredient provided for centrifuge.");
            return null;
        }

        List<Essence> possibleEssences = new List<Essence>();

        CheckAndAddEssence(possibleEssences, EssenceType.Alcohol, ingredient.Stats.Alcohol);
        CheckAndAddEssence(possibleEssences, EssenceType.Toxicity, ingredient.Stats.Toxicity);
        CheckAndAddEssence(possibleEssences, EssenceType.Sweetness, ingredient.Stats.Sweetness);
        CheckAndAddEssence(possibleEssences, EssenceType.Bitterness, ingredient.Stats.Bitterness);
        CheckAndAddEssence(possibleEssences, EssenceType.Sourness, ingredient.Stats.Sourness);
        CheckAndAddEssence(possibleEssences, EssenceType.Uniqueness, ingredient.Stats.Uniqueness);

        if (possibleEssences.Count > 0)
        {
            Essence selectedEssence = possibleEssences[Random.Range(0, possibleEssences.Count)];
            SOIngredient essenceIngredient = CreateEssence(selectedEssence);
            Debug.Log($"Created {selectedEssence.Quality} {selectedEssence.Type} essence!");
            return essenceIngredient;
        }
        else
        {
            Debug.Log("No essences can be extracted from this ingredient.");
            return null;
        }
    }

    private void CheckAndAddEssence(List<Essence> essences, EssenceType type, float value)
    {
        EssenceQuality quality = GetEssenceQuality(value);
        if (quality != EssenceQuality.Weak)
        {
            essences.Add(_essences.First(e => e.Type == type && e.Quality == quality));
        }
    }

    private EssenceQuality GetEssenceQuality(float value)
    {
        if (value >= 0.75f) return EssenceQuality.Epic;
        if (value >= 0.5f) return EssenceQuality.Strong;
        if (value >= 0.25f) return EssenceQuality.Decent;
        return EssenceQuality.Weak;
    }

    private SOIngredient CreateEssence(Essence essence)
    {
        SOIngredient essenceIngredient = ScriptableObject.CreateInstance<SOIngredient>();
        //essenceIngredient.Name = $"{essence.Quality} {essence.Type} Essence";
        //essenceIngredient.Icon = essence.Icon;

        // Устанавливаем характеристики эссенции
        essenceIngredient.Stats = new Stats();
        switch (essence.Type)
        {
            case EssenceType.Alcohol:
                essenceIngredient.Stats.Alcohol = GetEssenceValue(essence.Quality);
                break;
            case EssenceType.Toxicity:
                essenceIngredient.Stats.Toxicity = GetEssenceValue(essence.Quality);
                break;
            case EssenceType.Sweetness:
                essenceIngredient.Stats.Sweetness = GetEssenceValue(essence.Quality);
                break;
            case EssenceType.Bitterness:
                essenceIngredient.Stats.Bitterness = GetEssenceValue(essence.Quality);
                break;
            case EssenceType.Sourness:
                essenceIngredient.Stats.Sourness = GetEssenceValue(essence.Quality);
                break;
            case EssenceType.Uniqueness:
                essenceIngredient.Stats.Uniqueness = GetEssenceValue(essence.Quality);
                break;
        }

        return essenceIngredient;
    }

    private float GetEssenceValue(EssenceQuality quality)
    {
        switch (quality)
        {
            case EssenceQuality.Decent: return 0.5f;
            case EssenceQuality.Strong: return 0.75f;
            case EssenceQuality.Epic: return 1f;
            default: return 0.25f;
        }
    }
}