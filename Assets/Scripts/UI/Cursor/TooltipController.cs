using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum PriceType { Silver, Gold }

public class TooltipController : MonoBehaviour
{
    public static TooltipController Instance;

    [SerializeField] private OutlinedText _priceText;
    [SerializeField] private Image _priceImage;
    [SerializeField] private Sprite _goldSprite;
    [SerializeField] private Color _silverColor;
    [SerializeField] private Color _goldColor;

    private Sprite _silverSprite;

    private TextMeshProUGUI[] _texts;
    private RectTransform _rect;
    private CanvasGroup _group;
    private CanvasGroup _priceGroup;

    public RectTransform Rect { get { return _rect; } }
    public float Width { get { return _rect.rect.width; } }
    public float Height { get { return _rect.rect.height; } }
    public bool Enabled { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else Instance = this;

        _rect = GetComponent<RectTransform>();
        _group = GetComponent<CanvasGroup>();
        _texts = GetComponentsInChildren<TextMeshProUGUI>();
        _priceGroup = _priceText.GetComponent<CanvasGroup>();
        _priceImage = _priceText.GetComponentInChildren<Image>();
        _silverSprite = _priceImage.sprite;
    }

    public void SetTooltip(string text, PriceType priceType = PriceType.Silver, int price=0)
    {
        Enabled = true;
        _group.alpha = 1;
        foreach (var t in _texts)
            t.text = text.RemoveBetween("<",">");
        _texts[4].text = text;

        _priceGroup.alpha = Mathf.Clamp01(price);
        _priceImage.sprite = priceType==PriceType.Silver? _silverSprite : _goldSprite;
        var color = priceType == PriceType.Silver ? _silverColor : _goldColor;
        _priceText.SetText($"<color=#{color.ToHexString()}>{price}" );
    }
    public void HideTooltip()
    {
        Enabled=false;
        _group.alpha = 0;
        _priceGroup.alpha = 0;
    }

    public void SetAlignment(HorizontalAlignmentOptions align)
    {
        foreach (var t in _texts)
            t.horizontalAlignment = align;
    }
    public void SetAlignment(VerticalAlignmentOptions align)
    {
        foreach (var t in _texts)
            t.verticalAlignment = align;
    }

    public void AdjustPosition(RectTransform parent)
    {
        if (!Enabled) return;
        Vector2 pos = parent.anchoredPosition;

        if (parent.anchoredPosition.x + Width > 320)
        {
            SetAlignment(HorizontalAlignmentOptions.Right);
            pos = new Vector2(-81.5f, pos.y);
            //AlignOutline(2, 1);
        }
        else
        {
            pos = new Vector2(4.6f, pos.y);
            SetAlignment(HorizontalAlignmentOptions.Left);
            //AlignOutline(1, 1);
        }

        if (parent.anchoredPosition.y - Height < 0)
        {
            SetAlignment(VerticalAlignmentOptions.Bottom);
            pos = new Vector2(pos.x, 42.6f);
            //AlignOutline(0, 2);
        }
        else
        {
            SetAlignment(VerticalAlignmentOptions.Top);
            pos = new Vector2(pos.x, 7.4f);
            //AlignOutline(0, 1);
        }

        Rect.anchoredPosition = pos;
    }
}
