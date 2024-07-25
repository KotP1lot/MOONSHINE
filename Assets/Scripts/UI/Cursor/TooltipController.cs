using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TooltipController : MonoBehaviour
{
    public static TooltipController Instance;

    private TextMeshProUGUI[] _texts;
    private RectTransform _rect;
    private CanvasGroup _group;

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
    }

    public void SetTooltip(string text)
    {
        Enabled = true;
        _group.alpha = 1;
        foreach (var t in _texts)
            t.text = text.RemoveBetween("<",">");
        _texts[4].text = text;
    }
    public void HideTooltip()
    {
        Enabled=false;
        _group.alpha = 0;
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
    public void AlignOutline(float xmod , float ymod)
    {
        foreach (var t in _texts)
            t.rectTransform.anchoredPosition = new Vector2(
                xmod == 0 ? t.rectTransform.anchoredPosition.x : Mathf.Clamp(t.rectTransform.anchoredPosition.x, -1, 1) * xmod, 
                Mathf.Clamp(t.rectTransform.anchoredPosition.y, -1, 1) * ymod
                );
    }

    public void AdjustPosition(RectTransform parent)
    {
        if (!Enabled) return;
        Vector2 pos = parent.anchoredPosition;

        if (parent.anchoredPosition.x + Width > 320)
        {
            SetAlignment(HorizontalAlignmentOptions.Right);
            pos = new Vector2(-60.6f, pos.y);
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
