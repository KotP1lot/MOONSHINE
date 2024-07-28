using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Centrifuge : Aparat, IUpgrade
{
    [Header("Upgrade")]
    [SerializeField] private SOUpgrade _so;

    [Header("Essence Prefabs")]
    [SerializeField] private EssenceComponent alcoholEssencePrefab;
    [SerializeField] private EssenceComponent toxicityEssencePrefab;
    [SerializeField] private EssenceComponent sweetnessEssencePrefab;
    [SerializeField] private EssenceComponent bitternessEssencePrefab;
    [SerializeField] private EssenceComponent sournessEssencePrefab;

    [Space(10)]
    [SerializeField] private Transform _lever;
    [SerializeField] private SpriteRenderer _cover;
    [SerializeField] private Transform _outputPoint;
    [SerializeField] private Collider _ceiling;

    private Collider2D _leverCollider;
    private Collider _collider;
    private List<Ingredient> _ingredients = new List<Ingredient>();
    private ErrorCanvas _error;

    public Action<bool, int> OnUnlock { get; set; }

    private void Start()
    {
        _so.OnUpgrade += OnUpgrade;
        _collider = GetComponent<Collider>();
        _leverCollider = _lever.GetComponent<Collider2D>();
        _error= GetComponentInChildren<ErrorCanvas>();
        gameObject.SetActive(false);
    }
    private void OnUpgrade(Upgrade upgrade) 
    {
        if (upgrade.CurrLvl == 0) OnUnlock?.Invoke(true, 2);

    }
    private void OnTriggerEnter(Collider other)
    {
        var ingredient = other.GetComponentInParent<Ingredient>();

        if (ingredient != null)
        {
            _ingredients.Add(ingredient);
        }
    }

    private EssenceComponent ProcessIngredient(Stats stats)
    {
        Essence essence = CreateEssenceFromIngredient(stats);

        var essenceObject = CreateEssenceObject(essence);
        if (essenceObject != null)
        {
            essenceObject.transform.position = _outputPoint.position;
            essenceObject.transform.localScale = Vector3.zero;
            essenceObject.transform.rotation = Quaternion.identity;

        }

        return essenceObject;
    }

    private Essence CreateEssenceFromIngredient(Stats stats)
    {
        var index = stats.GetHighestStatIndex();

        Essence essence = ScriptableObject.CreateInstance<Essence>();
        essence.Type = (StatType)index;
        essence.Strength = stats.Array[index] / GameManager.Instance.HighestStat * GameManager.Instance.HighestEssencePercent;

        return essence;
    }

    private EssenceComponent CreateEssenceObject(Essence essence)
    {
        EssenceComponent prefab = essence.Type switch
        {
            StatType.Alcohol => alcoholEssencePrefab,
            StatType.Toxicity => toxicityEssencePrefab,
            StatType.Sweetness => sweetnessEssencePrefab,
            StatType.Bitterness => bitternessEssencePrefab,
            StatType.Sourness => sournessEssencePrefab,
            _ => null
        };

        if (prefab != null)
        {
            EssenceComponent essenceComponent = Instantiate(prefab);
            essenceComponent.SetEssence(essence);
            return essenceComponent;
        }

        Debug.LogError("No prefab found for the essence type.");
        return null;
    }

    public void TryExtract()
    {
        _collider.enabled = true;
        _leverCollider.enabled = false;
        Utility.Delay(0.1f, () => 
        {
            if (_ingredients.Count == 1)
            {
                AudioManager.instance.Play("Lever");
                if (_ingredients[0].Data.IsEnhanced)
                {
                    _error.ShowText("can't extract from enhanced ingredient");
                    AudioManager.instance.Play("Error");
                    CancelExtraction();
                }
                else StartExtraction(_ingredients[0]);
            }
            else
            {
                if (_ingredients.Count > 1) _error.ShowText("more than one ingredient in the tank");
                if (_ingredients.Count == 0) _error.ShowText("tank is empty");
                AudioManager.instance.Play("Error");
                CancelExtraction();
            }
        });
    }

    private void StartExtraction(Ingredient ingredient)
    {
        GameManager.Instance.SetProcessing(true);
        _collider.enabled = false;
        _ceiling.enabled = true;
        _lever.DORotate(new Vector3(0, 0, -27), 0.3f).SetEase(Ease.OutBack, 2);

        AudioManager.instance.Play("Centrifuge");
        var essence = ProcessIngredient(ingredient.GetStats());

        ingredient.EnablePhysics(false);
        ingredient.transform.DOShakePosition(2, 0.2f, 20,90,false,false).SetEase(Ease.InCirc)
            .onComplete = () => Destroy(ingredient.gameObject);

        _cover.DOFade(1,1.3f).SetEase(Ease.InSine).SetDelay(0.5f);

        Utility.Delay(2.5f, () =>
        {
            essence.transform.DOScale(1, 2).SetEase(Ease.OutQuad);
            _cover.transform.DOScaleY(0, 2).SetEase(Ease.OutQuad);

            Utility.Delay(2.2f, () => 
            {
                essence.EnablePhysics(true);
                AudioManager.instance.Play("Pop");

                GameManager.Instance.SetProcessing(false);
                ResetCentrifuge();
            });
        });
    }

    private void CancelExtraction()
    {
        _collider.enabled = false;
        _lever.DORotate(new Vector3(0, 0, 17), 0.2f).SetEase(Ease.OutCirc, 2).onComplete = () =>
        {
            ResetCentrifuge();
        };
    }

    private void ResetCentrifuge()
    {
        _cover.DOFade(0, 0);
        _cover.transform.localScale = Vector3.one;

        _lever.DORotate(new Vector3(0, 0, 27), 0.2f).SetEase(Ease.OutBack, 3).onComplete = () =>
                _leverCollider.enabled = true;

        _ceiling.enabled = false;
        _ingredients.Clear();
    }
}
