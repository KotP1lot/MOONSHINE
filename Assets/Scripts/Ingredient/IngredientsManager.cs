using System;
using System.Collections.Generic;
using UnityEngine;

public class IngredientsManager : MonoBehaviour
{
    [SerializeField] private List<SOIngredient> _unlocked = new();
    [SerializeField] private List<SOIngredient> _all = new();

    private void Start()
    {
    }

    public void AddIngredient(SOIngredient ingredient) => _unlocked.Add(ingredient);

    public void UnlockRandomIngredients(int count)
    {
        Utility.ShuffleList(_all);
        _unlocked.AddRange(_all.GetRange(0, count));
    }

    public List<SOIngredient> GetRandomIngredients(int count)
    {
        Utility.ShuffleList(_unlocked);
        return _unlocked.GetRange(0, count);
    }

    public List<SOIngredient> GetAllIngredients()
    {
        return new List<SOIngredient>(_all);
    }

    public void UnlockIngredient(SOIngredient ingredient)
    {
        if (!ingredient.Unlocked)
        {
            _unlocked.Add(ingredient);
            ingredient.Unlocked = true;
        }
    }

    public Essence DestroyIngredient(Ingredient ingredient)
    {
        // Get the stats of the ingredient
        Stats stats = ingredient.GetStats();

        // Randomly select an essence type
        Array values = Enum.GetValues(typeof(Essence.EssenceType));
        Essence.EssenceType randomEssenceType = (Essence.EssenceType)values.GetValue(UnityEngine.Random.Range(0, values.Length));

        // Determine the strength of the essence based on the selected type
        float strength = randomEssenceType switch
        {
            Essence.EssenceType.Alcohol => stats.Alcohol,
            Essence.EssenceType.Toxicity => stats.Toxicity,
            Essence.EssenceType.Sweetness => stats.Sweetness,
            Essence.EssenceType.Bitterness => stats.Bitterness,
            Essence.EssenceType.Sourness => stats.Sourness,
            _ => 0f,
        };

        // Create the essence
        Essence essence = ScriptableObject.CreateInstance<Essence>();
        essence.Type = randomEssenceType;
        essence.Strength = strength;

        // Mark the ingredient as used
        ingredient.IsUsed = true;

        return essence;
    }
}
