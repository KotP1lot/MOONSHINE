using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,IPointerUpHandler
{
    [SerializeField] private CursorType _Hover;
    [SerializeField] private CursorType _Down;
    private bool _isUI;
    private bool _isHovering;

    private void Update()
    {
        _isUI = TryGetComponent<RectTransform>(out var rect);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorController.Instance.SetSprite(_Hover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorController.Instance.SetSprite();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        CursorController.Instance.SetSprite(_Down);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerExit(null);
    }

    private void OnMouseEnter()
    {
        if (!_isUI) OnPointerEnter(null);
    }

    private void OnMouseExit()
    {
        if (!_isUI) OnPointerExit(null);
    }
    private void OnMouseDrag()
    {
        if (!_isUI) OnPointerDown(null);
    }
    private void OnMouseUp()
    {
        if (!_isUI) OnPointerExit(null);
    }

}
