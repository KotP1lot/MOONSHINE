using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,IPointerUpHandler
{
    [SerializeField] private CursorType _Hover;
    [SerializeField] private CursorType _Down;
    [Space(10)]
    [TextArea][SerializeField] private string _tooltip;
    private bool _isUI;

    public bool IsHovering { get; private set; }

    private void Update()
    {
        _isUI = TryGetComponent<RectTransform>(out var rect);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsHovering = true;
        CursorController.Instance.SetSprite(_Hover);
        if (_tooltip != "") TooltipController.Instance.SetTooltip(_tooltip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsHovering = false;
        CursorController.Instance.SetSprite();
        if (_tooltip != "") TooltipController.Instance.HideTooltip();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        IsHovering = true;
        CursorController.Instance.SetSprite(_Down);
        if (_tooltip != "") TooltipController.Instance.SetTooltip(_tooltip);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if(!IsHovering) OnPointerExit(null);
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

    public void SetTooltip(string text)
    {
        _tooltip = text;
    }
}
