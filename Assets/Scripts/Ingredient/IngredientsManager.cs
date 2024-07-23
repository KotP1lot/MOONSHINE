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

    //public void DestroyIngredient(Ingredient ingredient)
    //{
    //    // Уничтожаем ингредиент
    //    if (ingredient != null)
    //    {
    //        Destroy(ingredient.gameObject);
    //    }
    //}

    //public Essence CreateEssenceFromIngredient(Ingredient ingredient)
    //{
    //    // Получаем характеристики ингредиента
    //    Stats stats = ingredient.GetStats();

    //    // Случайным образом выбираем тип эссенции
    //    Array values = Enum.GetValues(typeof(Essence.EssenceType));
    //    Essence.EssenceType randomEssenceType = (Essence.EssenceType)values.GetValue(UnityEngine.Random.Range(0, values.Length));

    //    // Определяем силу эссенции в зависимости от выбранного типа
    //    float strength = randomEssenceType switch
    //    {
    //        Essence.EssenceType.Alcohol => stats.Alcohol,
    //        Essence.EssenceType.Toxicity => stats.Toxicity,
    //        Essence.EssenceType.Sweetness => stats.Sweetness,
    //        Essence.EssenceType.Bitterness => stats.Bitterness,
    //        Essence.EssenceType.Sourness => stats.Sourness,
    //        _ => 0f,
    //    };

    //    // Создаём эссенцию
    //    Essence essence = ScriptableObject.CreateInstance<Essence>();
    //    essence.Type = randomEssenceType;
    //    essence.Strength = strength;

    //    return essence;
    //}

}
