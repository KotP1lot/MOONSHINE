using System.Collections.Generic;
using UnityEngine;

public class IngredientsManager : MonoBehaviour
{
    [SerializeField]private List<SOIngredient> _unlocked = new();
    [SerializeField]private List<SOIngredient> _all = new();
    private void Start()
    {

    }
    public void AddIngridient(SOIngredient ingredient) => _unlocked.Add(ingredient);
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
}
