using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct StatColors
{
    public Color Alcohol;
    public Color Toxicity;
    public Color Sweetness;
    public Color Bitterness;
    public Color Sourness;
    public Color NegativeValue;

    public Color[] Array
    {
        get
        {
            return new[] { Alcohol, Toxicity, Sweetness, Bitterness, Sourness };
        }
    }
}

[Serializable]
public struct RarityPrice
{
    public Rarity Rarity;
    public int Price;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] ClientManager _clientManager;
    [SerializeField] UIPlayerStats _uiPlayerStats;
    [SerializeField] int _maxStars;
    [SerializeField] AparatChanger _aparatChanger;

    [Header("Stats")]
    public int HighestStat;
    public float HighestEssencePercent;
    public StatColors Colors;

    [Header("Prices")]
    [SerializeField] private int _essenceTopCost;
    [SerializeField] private RarityPrice[] _rarityPrices;

    public Value Silver { get; private set; }
    public Value Gold { get; private set; }
    public Value Stars { get; private set; }
    public Value Days { get; private set; }
    public bool IsPlayState { get; private set; }

    private Resetter[] _resetters;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        _clientManager.OnDayEnd += OnDayEnded;
        _clientManager.OnGetStar += GetStars;
        Silver = new();
        Gold = new();
        Stars = new();
        Days = new();

        _resetters = FindObjectsOfType<MonoBehaviour>().OfType<Resetter>().ToArray();
    }
    private void OnDisable()
    {
        _clientManager.OnDayEnd -= OnDayEnded;
        _clientManager.OnGetStar -= GetStars;
    }
    private void Start()
    {
        Gold.AddAmount(30);
        StartNewDay();
    }
    private void OnDayEnded() 
    {
        IsPlayState = false;
    }
    private void StartNewDay()
    {
        IsPlayState = true;
        Days.AddAmount(1);
        Silver.ChangeValue(100);
        _clientManager.StartNewDay(10, 10, 0); 
    }
    public void GetStars(int value)
    {
        Stars.AddAmount(value);
        if (Stars.Amount >= _maxStars) 
        {
            GlobalEvents.Instance.OnGameEnded?.Invoke();
            IsPlayState = false;
            return;
        }
    }

    public void SetProcessing(bool proc)
    {
        _aparatChanger.EnableButtons(!proc);
    }

    public void ResetAll()
    {
        foreach (var reset in _resetters)
            reset.ResetValues();
    }

    public int GetPriceByRarity(Rarity rarity)
    {
        return _rarityPrices.First(x=>x.Rarity == rarity).Price;
    }

    public int GetEssenceCost(float strength)
    {
        return Mathf.RoundToInt(strength/HighestEssencePercent * _essenceTopCost);
    }
}
