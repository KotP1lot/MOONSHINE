using System;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    [SerializeField] IngredientsManager _ingredientsManager;
    [SerializeField] int _width;
    [SerializeField] GameObject _posPrefab;
    [SerializeField] Ingredient _³ngredientPrefab;
    [SerializeField] int _cObjects;
    [SerializeField] int _cRefresh;


    private List<Ingredient> _ingredients = new();
    private List<Transform> _positions = new();
    void Start()
    {
        float step = (1 * _width) / ((float)_cObjects - 1);
        for (int i = 0; i < _cObjects; i++)
        {
            GameObject obj = Instantiate(_posPrefab, transform);
            obj.transform.localPosition = new Vector2(i * step - _width / 2, 1f);
            _positions.Add(obj.transform);
        }
        RefreshShelf();
    }
    public void RefreshShelf()
    {
        if (_cRefresh-- <= 0) return;
        SetIngredients(_ingredientsManager.GetRandomIngredients(_cObjects));
    }

    public void SetIngredients(List<SOIngredient> newIngredients)
    {
        while (_ingredients.Count < _cObjects)
        {
            Ingredient ingredient = Instantiate(_³ngredientPrefab, GetEmptyPos());
            ingredient.OnParentChange += Ingredient_OnParentChange;
            ingredient.OnClick += Ingredient_OnClick;
            _ingredients.Add(ingredient);
        }

        for (int i = 0; i < newIngredients.Count; i++)
        {
            _ingredients[i].Setup(newIngredients[i]);
            _ingredients[i].ResetLocalPosition();
        }
    }

    private void Ingredient_OnClick(Ingredient ingredient)
    {
        ingredient.transform.parent = null;
        ingredient.transform.rotation = Quaternion.identity;
        ingredient.OnParentChange -= Ingredient_OnParentChange;
        ingredient.OnClick -= Ingredient_OnClick;
        RemoveFromList(ingredient);
    }

    public void RemoveFromList(Ingredient ingredient)
    {
        _ingredients.Remove(ingredient);
    }
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
