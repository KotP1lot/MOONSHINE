using System.Collections.Generic;
using UnityEngine;

public class IngredientsManager : MonoBehaviour
{
    [SerializeField] private List<ScriptableObject> _unlocked = new();
    [SerializeField] private List<ScriptableObject> _all = new();

    public void AddIngredient(ScriptableObject ingredient) => _unlocked.Add(ingredient);

    public void UnlockRandomIngredients(int count)
    {
        Utility.ShuffleList(_all);
        _unlocked.AddRange(_all.GetRange(0, count));
    }

    public List<ScriptableObject> GetRandomIngredients(int count)
    {
        Utility.ShuffleList(_unlocked);
        return _unlocked.GetRange(0, count);
    }

    public List<ScriptableObject> GetAllIngredients()
    {
        return new List<ScriptableObject>(_all);
    }

    public bool UnlockIngredient(ScriptableObject ingredient,int cashback)
    {
        if (!_unlocked.Contains(ingredient))
        {
            _unlocked.Add(ingredient);
            return true;
        }
        else
        {
            GameManager.Instance.Gold.AddAmount(cashback);
        }
        return false;
    }
}
