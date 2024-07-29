using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStats : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _money;
    [SerializeField] TextMeshProUGUI _silver;
    [SerializeField] TextMeshProUGUI _days;

    [SerializeField] UIStar _starPref;
    [SerializeField] Image _stars;

    private void Start()
    {
        GameManager.Instance.Gold.OnResourceChanged += OnMoneyChanged;
        GameManager.Instance.Silver.OnResourceChanged += OnSilverChanged;
        GameManager.Instance.Stars.OnResourceChanged += OnStarChanged;
        GameManager.Instance.Days.OnResourceChanged += OnDaysChanged;
    }
    private void OnDisable()
    {
        GameManager.Instance.Gold.OnResourceChanged -= OnMoneyChanged;
        GameManager.Instance.Silver.OnResourceChanged -= OnMoneyChanged;
        GameManager.Instance.Stars.OnResourceChanged -= OnStarChanged;
        GameManager.Instance.Days.OnResourceChanged -= OnDaysChanged;
    }
    public void OnSilverChanged(int value) 
    {
        _silver.text = $"{value}";
    }
    public void OnMoneyChanged(int value) 
    {
        _money.text = $"{value}";
    }
    public void OnDaysChanged(int value) 
    {
        _days.text = $"{value}";
    }
    public void OnStarChanged(int value) 
    {
        _stars.DOFillAmount((float)value/3, 0.3f).SetEase(Ease.OutCirc);
    }
}
