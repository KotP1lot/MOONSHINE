using System.Collections.Generic;
using UnityEngine;

public class IngredientsManager : MonoBehaviour
{
    [SerializeField]private List<SOIngredient> _avalible = new();
    [SerializeField]private List<SOIngredient> _all = new();
    private void Start()
    {
        UnlockRandomIngredients(4);
    }
    public void AddIngridient(SOIngredient ingredient) => _avalible.Add(ingredient);
    public void UnlockRandomIngredients(int count)
    {
        Utility.ShuffleList(_all);
        _avalible.AddRange(_all.GetRange(0, count));
    }
    public List<SOIngredient> GetRandomIngredients(int count) 
    {
        Utility.ShuffleList(_avalible);
        return _avalible.GetRange(0, count);
    }
}
