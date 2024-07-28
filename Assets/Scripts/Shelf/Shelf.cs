using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    [Header("Upgrade")]
    [SerializeField] SOUpgrade _so;
    [SerializeField] SpriteRenderer _lockedSprite;

    [Space(10)]
    [SerializeField] IngredientsManager _ingredientsManager;
    [SerializeField] float _width;
    [SerializeField] GameObject _posPrefab;
    [SerializeField] Item _³ngredientPrefab;
    [SerializeField] int _maxItemCount;
    [SerializeField] int _itemCount;
    [SerializeField] int _cRefresh;
    [Space(10)]
    [SerializeField] float _depth;
    [SerializeField] bool _isIngredient;


    private List<Item> _items = new();
    private List<Transform> _positions = new();
    void Start()
    {
        if (_so != null) SOSetup();
        GlobalEvents.Instance.OnBeerCooked += ResetShelf;

        float step = (1 * _width) / ((float)_maxItemCount - 1); 
        for (int i = 0; i < _maxItemCount; i++)
        {
            GameObject obj = Instantiate(_posPrefab, transform);
            obj.transform.localPosition = new Vector3(i * step - _width / 2, 1f, _depth);
            _positions.Add(obj.transform);
        }
        if (_so == null) RefreshShelf();
    }
    private void SOSetup() 
    {
        _so.OnUpgrade += UpgradeShelf;
    }
    private void UnlockShelf() 
    {
        _lockedSprite.sprite = null;
    }
    private void UpgradeShelf(Upgrade upgrade) 
    {
        if(upgrade.CurrLvl == 0) UnlockShelf();
        _itemCount = _so.LvlInfo[upgrade.CurrLvl].bonus;
    }
    private void ResetShelf()
    {
        _items.Clear();
        RefreshShelf();
    }

    public void RefreshShelf()
    {
        if (_cRefresh-- <= 0) return;
        if (_isIngredient) SetItems(_ingredientsManager.GetRandomIngredients(_itemCount));
        else SetItems(GenerateEssense(_itemCount));
    }
    private List<ScriptableObject> GenerateEssense(int count) 
    {
        List<ScriptableObject> objects = new();
        float highestEssencePercent = GameManager.Instance.HighestEssencePercent;
        while (objects.Count < count)
        {
            int index = Random.Range(0, 5);

            Essence essence = ScriptableObject.CreateInstance<Essence>();
            essence.Type = (StatType)index;

            essence.Strength = Random.Range(0, highestEssencePercent);

            objects.Add(essence);
        }
        return objects;
    }

    public void SetItems(List<ScriptableObject> newIngredients)
    {
        while (_items.Count < _itemCount)
        {
            Item item = Instantiate(_³ngredientPrefab, GetEmptyPos());
            item.SetLayer(LayerMask.NameToLayer("OnShelf"));
            item.OnParentChange += Ingredient_OnParentChange;
            item.OnClick += Ingredient_OnClick;
            _items.Add(item);
        }

        for (int i = 0; i < newIngredients.Count; i++)
            _items[i].Setup(newIngredients[i]);
    }
    private void OnDisable()
    {
        _items.ForEach(x =>
        {
            x.OnParentChange -= Ingredient_OnParentChange;
            x.OnClick -= Ingredient_OnClick;
        });
        GlobalEvents.Instance.OnBeerCooked -= ResetShelf;
    }
    private void Ingredient_OnClick(Item item)
    {
        item.transform.parent = null;
        item.transform.rotation = Quaternion.identity;
        item.SetLayer(LayerMask.NameToLayer("Default"));
        item.transform.position -= new Vector3(0, 0, item.transform.position.z);
        item.OnParentChange -= Ingredient_OnParentChange;
        item.OnClick -= Ingredient_OnClick;
        RemoveFromList(item);
        AudioManager.instance.Play("BuyIngredient");
    }

    public void RemoveFromList(Item ingredient)
    {
        _items.Remove(ingredient);
    }
    private Transform GetEmptyPos()
    {
        return _positions.Find(x => x.childCount == 0);
    }
    private void Ingredient_OnParentChange(Item obj)
    {
        obj.OnParentChange -= Ingredient_OnParentChange;
        RemoveFromList(obj);
    }
}
