using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPlayerStats : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _money;
    [SerializeField] TextMeshProUGUI _days;

    [SerializeField] UIStar _starPref;
    List<UIStar> _stars = new();

    public void Setup(int stars) 
    {

        GameManager.Instance.Gold.OnResourceChanged += OnMoneyChanged;
        GameManager.Instance.Stars.OnResourceChanged += OnStarChanged;
        GameManager.Instance.Days.OnResourceChanged += OnDaysChanged;
    }
    private void OnDisable()
    {
        GameManager.Instance.Gold.OnResourceChanged -= OnMoneyChanged;
        GameManager.Instance.Stars.OnResourceChanged -= OnStarChanged;
        GameManager.Instance.Days.OnResourceChanged -= OnDaysChanged;
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
