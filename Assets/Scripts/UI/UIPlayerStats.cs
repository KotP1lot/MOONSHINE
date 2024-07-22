using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPlayerStats : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _money;
    [SerializeField] TextMeshProUGUI _days;

    [SerializeField] Transform _starContainer;
    [SerializeField] UIStar _starPref;
    List<UIStar> _stars = new();

    public void Setup(int stars) 
    {
        for (int i = 0; i < stars; i++)
        {
            _stars.Add(Instantiate(_starPref, _starContainer));
        }
        GameManager.Instance.Gold.OnResourceChanged += OnMoneyChanged;
        GameManager.Instance.Stars.OnResourceChanged += OnStarChanged;
        GameManager.Instance.Days.OnResourceChanged += OnDaysChanged;
    }
    public void OnMoneyChanged(int value) 
    {
        _money.text = $"${value}";
    }
    public void OnDaysChanged(int value) 
    {
        _days.text = $"Day: {value}";
    }
    public void OnStarChanged(int value) 
    {
        Debug.Log("UI");
        for(int i = 0; i<value; i++) 
        {
            _stars[i].SetActive(true);
        }
    }
}
