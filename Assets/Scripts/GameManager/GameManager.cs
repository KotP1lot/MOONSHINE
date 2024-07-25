using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] ClientManager _clientManager;
    [SerializeField] UIPlayerStats _uiPlayerStats;
    [SerializeField] int _maxStars;
    public Value Silver { get; private set; }
    public Value Gold { get; private set; }
    public Value Stars { get; private set; }
    public Value Days { get; private set; }

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
        _clientManager.OnDayEnd += StartNewDay;
        _clientManager.OnGetStar += GetStars;
        Silver = new();
        Gold = new();
        Stars = new();
        Days = new();
    }
    private void OnDisable()
    {
        _clientManager.OnDayEnd -= StartNewDay;
        _clientManager.OnGetStar -= GetStars;
    }
    private void Start()
    {
        _uiPlayerStats.Setup(_maxStars);
        Gold.AddAmount(10);
        StartNewDay();
    }
    private void StartNewDay() 
    {
        Days.AddAmount(1);
        Silver.ChangeValue(10);
        _clientManager.StartNewDay(10, 2, 2); 
    }
    public void GetStars(int value)
    {
        Debug.Log("GameManager");
        Stars.AddAmount(value);
        if (Stars.Amount >= _maxStars) 
        {
            Debug.Log("KINEZ");
            return;
        }
    }
}
