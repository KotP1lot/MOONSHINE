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
    [Space(10)]
    [SerializeField] float _depth;


    private List<Ingredient> _ingredients = new();
    private List<Transform> _positions = new();
    void Start()
    {
        GlobalEvents.Instance.OnBeerCooked += ResetShelf;

        float step = (1 * _width) / ((float)_cObjects - 1);
        for (int i = 0; i < _cObjects; i++)
        {
            GameObject obj = Instantiate(_posPrefab, transform);
            obj.transform.localPosition = new Vector3(i * step - _width / 2, 1f, _depth);
            _positions.Add(obj.transform);
        }
        RefreshShelf();
    }

    private void ResetShelf()
    {
        _ingredients.Clear();
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
            ingredient.SetLayer(LayerMask.NameToLayer("OnShelf"));
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
    private void OnDisable()
    {
        _ingredients.ForEach(x =>
        {
            x.OnParentChange -= Ingredient_OnParentChange;
            x.OnClick -= Ingredient_OnClick;
        });
        GlobalEvents.Instance.OnBeerCooked -= ResetShelf;
    }
    private void Ingredient_OnClick(Ingredient ingredient)
    {
        ingredient.transform.parent = null;
        ingredient.transform.rotation = Quaternion.identity;
        ingredient.SetLayer(LayerMask.NameToLayer("Default"));
        ingredient.transform.position -= new Vector3(0, 0, ingredient.transform.position.z);
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
