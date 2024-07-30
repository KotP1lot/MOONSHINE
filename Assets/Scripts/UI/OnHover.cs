using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnHover : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private UnityEvent _onHover;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _onHover?.Invoke();
    }

    private CanvasGroup _canvasGroup;
    
    void Start()
    {
        _canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    
    public void ShowWindow(bool show)
    {
        if (_canvasGroup == null) return;
        _canvasGroup.DOComplete();
        _canvasGroup.DOFade(show ? 1 : 0, 0.4f).SetEase(Ease.OutCirc);
        _canvasGroup.interactable = show;
        _canvasGroup.blocksRaycasts = show;
    }
}
