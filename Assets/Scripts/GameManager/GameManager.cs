using System;
using System.Collections.Generic;
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

[Serializable]
public struct GradeGold 
{
    public GradeType Grade;
    public int Gold;
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] ClientManager _clientManager;
    [SerializeField] int _maxStars;
    [SerializeField] AparatChanger _aparatChanger;

    [Header("Stats")]
    public int HighestStat;
    public float HighestEssencePercent;
    public StatColors Colors;

    [Header("Prices")]
    [SerializeField] private int _essenceTopCost;
    [SerializeField] private RarityPrice[] _rarityPrices;

    [Header("Days")]
    [SerializeField] List<Day> _days;
    private Queue<Day> _daysQueue;
    public Day CurrentDay { get; private set; }


    [Header("Macro")]
    [SerializeField] private int _silverPerClient;
    [SerializeField] private int _startGoldAmount;
    [SerializeField] private List<GradeGold> _gradeGold;
    private GradeType _reputation;
    private int _gradeCount;
    private int _grade;
    
    
    public Value Silver { get; private set; }
    public Value Gold { get; private set; }
    public Value Stars { get; private set; }
    public Value Days { get; private set; }
    public bool IsPlayState { get; private set; }


    private Resetter[] _resetters;

    public int GetBribe() 
    {
        return UnityEngine.Random.Range(CurrentDay.MinBribe, CurrentDay.MaxBribe + 1);
    }
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
        Gold.AddAmount(_startGoldAmount);
        _daysQueue = new Queue<Day>(_days);
    }

    //TEMP --------------------------------------------------------------------------------------
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !IsPlayState)
            StartNewDay();
    }
    private void ShowMenu() 
    {
        //показувати меню між днями.    
    }
    private void OnDayEnded() 
    {
        if (_daysQueue.Count == 0) { /*кінець гри*/ }
        IsPlayState = false;
    }
    //TEMP --------------------------------------------------------------------------------------
    private void StartNewDay()
    { 
        CurrentDay = _daysQueue.Dequeue();
        IsPlayState = true;
        Days.AddAmount(1);
        Silver.ChangeValue(_silverPerClient);
        _clientManager.StartNewDay(CurrentDay.ClientCount, 
            CurrentDay.PoliceCount,
            CurrentDay.UnderoverPoliceCount); 
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

    public void SetNewGrade(GradeType grade)
    {

        Gold.AddAmount(_gradeGold.Find(x => x.Grade == grade).Gold);
        _grade += (int)grade;
        _gradeCount++;
        _reputation = (GradeType) Mathf.RoundToInt((float)_grade / (float)_gradeCount);
        Debug.Log(_reputation);
    }
}
