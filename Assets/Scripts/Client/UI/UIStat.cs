using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIStat : MonoBehaviour
{
    private StatWindow _statWindow;
    private CanvasGroup _canvasGroup;

    private void Start()
    {
        _statWindow = GetComponentInChildren<StatWindow>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    public void ShowStats(List<Stat> stats)
    {
        SetActive(true);
        _statWindow.SetLimits(stats);
    }
    public void SetActive(bool isActive)
    {
        _canvasGroup.DOFade(isActive? 1 : 0,0.3f).SetEase(Ease.OutCirc);
    }
    public void ShowValues(Stats stats,float duration = 0.4f)
    {
        _statWindow.SetStats(stats,duration);
    }
}
