using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,IPointerUpHandler
{
    [SerializeField] private CursorType _Hover;
    [SerializeField] private CursorType _Down;
    [Space(10)]
    [SerializeField] private PriceType _priceType;
    [SerializeField] private int _price;
    [Space(10)]
    [SerializeField] private bool _oneTimeClick = true;
    [SerializeField] private bool _clickSFX;
    [SerializeField] private bool _hoverSFX;
    [Space(10)]
    [TextArea][SerializeField] private string _tooltip;
    private bool _isUI;

    public bool IsHovering { get; private set; }

    public Action OnHover;

    private void Update()
    {
        _isUI = TryGetComponent<RectTransform>(out var rect);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_hoverSFX) AudioManager.instance.Play("UIHover");
        OnHover?.Invoke();
        IsHovering = true;
        CursorController.Instance.SetSprite(_Hover);
        if (_tooltip != "") TooltipController.Instance.SetTooltip(_tooltip,_priceType,_price);
        if (_tooltip == "resetTooltip") TooltipController.Instance.SetTooltip("");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsHovering = false;
        CursorController.Instance.SetSprite();
        if (_tooltip != "") TooltipController.Instance.HideTooltip();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(_clickSFX)AudioManager.instance.Play("Click");
        IsHovering = true;
        CursorController.Instance.SetSprite(_Down);
        if (_tooltip != ""&& _tooltip != "resetTooltip") TooltipController.Instance.SetTooltip(_tooltip, _priceType, _price);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if(_oneTimeClick) OnPointerExit(null);
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
        if (!_isUI) OnPointerUp(null);
    }

    public void SetTooltip(string text)
    {
        _tooltip = text;
    }
    public void SetPrice(PriceType priceType, int price)
    {
        _priceType = priceType;
        _price = price;
    }
    public void SetCursor(CursorType hover, CursorType down)
    {
        _Hover = hover;
        _Down = down;
    }
}
