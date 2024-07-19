using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    [SerializeField] IngredientsManager _ingredientsManager;
    [SerializeField] int _countOfObjects;
    [SerializeField] int _wigth;
    [SerializeField] GameObject _posPrefab;
    [SerializeField] Ingredient _³ngredientPrefab;
    private List<Ingredient> _ingredients = new();
    private List<Transform> _positions = new();
    void Start()
    {
        float step = (1 * _wigth) / ((float)_countOfObjects - 1);
        for (int i = 0; i < _countOfObjects; i++)
        {
            GameObject obj = Instantiate(_posPrefab, transform);
            obj.transform.localPosition = new Vector2(i * step - _wigth / 2, 0.5f);
            _positions.Add(obj.transform);
        }
        RefreshShelf();
    }
    public void RefreshShelf()
    {
        SetIngredients(_ingredientsManager.GetRandomIngredients(_countOfObjects));
    }

    public void SetIngredients(List<SOIngredient> newIngredients)
    {
        while (_ingredients.Count < _countOfObjects)
        {
            Ingredient ingredient = Instantiate(_³ngredientPrefab, GetEmptyPos());
            ingredient.transform.localPosition = Vector2.zero;
            ingredient.OnParentChange += Ingredient_OnParentChange;
            _ingredients.Add(ingredient);
        }

        for (int i = 0; i < newIngredients.Count; i++)
        {
            _ingredients[i].Setup(newIngredients[i]);
            _ingredients[i].SetRbType(RigidbodyType2D.Static);
            _ingredients[i].ResetLocalPosition();
        }
    }
    public void RemoveFromList(Ingredient ingredient) => _ingredients.Remove(ingredient);
    private Transform GetEmptyPos()
    {
        return _positions.Find(x => x.childCount == 0);
    }
    private void Ingredient_OnParentChange(Ingredient obj)
    {
        obj.OnParentChange -= Ingredient_OnParentChange;
        RemoveFromList(obj);
    }
}
