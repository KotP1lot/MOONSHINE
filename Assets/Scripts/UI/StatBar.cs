using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
    [SerializeField] private StatType _colorType;

    [HideInInspector]public float MaxValue;
    private float _fillValue;
    private float _lowLimitValue;
    private float _highLimitValue;
    private float _idealValue;

    private RectTransform _rectTransform;

    private Image _fillBar;
    private Image _lowLimit;
    private Image _highLimit;
    private Image _ideal;

    private TextMeshProUGUI _fillText;
    private CursorHover _cursorHover;
    private TextMeshProUGUI[] _tooltipTexts;
    private CanvasGroup _canvasGroup;

    private bool _isTweening;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _cursorHover = GetComponentInChildren<CursorHover>();
        _canvasGroup = _cursorHover.GetComponent<CanvasGroup>();
        _tooltipTexts = _cursorHover.GetComponentsInChildren<TextMeshProUGUI>();

        var images = GetComponentsInChildren<Image>();
        _lowLimit = images[2];
        _highLimit = images[3];
        _ideal = images[4];
        _fillBar = images[5];

        _fillBar.color = GameManager.Instance.Colors.Array[(int)_colorType];

        _fillText = _fillBar.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (_isTweening)
        {
            var fillPercent = Mathf.Clamp01(_fillBar.rectTransform.sizeDelta.x / _rectTransform.sizeDelta.x);
            if (_fillValue>MaxValue) fillPercent = _fillValue/MaxValue;
            SetValueText(fillPercent * MaxValue);
        }

        _canvasGroup.alpha = _cursorHover.IsHovering ? 1 : 0;
        if (_cursorHover.IsHovering)
        {
            var text = GenerateTooltip();
            foreach (var t in _tooltipTexts)
                t.text = text.RemoveBetween("<", ">");
            _tooltipTexts[4].text = text;
        }
    }

    private Vector3 PercentToPos(float percent)
    {
        return new Vector3(Mathf.RoundToInt(_rectTransform.sizeDelta.x * percent), 0, 0);
    }


    public void SetLimits(float low,float ideal, float high)
    {
        _lowLimitValue = low;
        _idealValue = ideal;
        _highLimitValue = high;

        _lowLimit.rectTransform.anchoredPosition = PercentToPos(_lowLimitValue / MaxValue);
        _highLimit.rectTransform.anchoredPosition = PercentToPos(_highLimitValue / MaxValue);
        _ideal.rectTransform.anchoredPosition = PercentToPos(_idealValue / MaxValue);
    }

    public void SetValue(float value)
    {
        _fillValue = value;

        var fillPercent = Mathf.Clamp01(_fillValue / MaxValue);
        _fillBar.rectTransform.DOKill();

        _isTweening = true;
        _fillBar.rectTransform.DOSizeDelta(new Vector3(PercentToPos(fillPercent).x,
            _fillBar.rectTransform.sizeDelta.y), 0.4f).SetEase(Ease.OutCirc)
            .onComplete = ()=> 
            {
                _isTweening = false;
                SetValueText(_fillValue);
            };

        
    }

    private void SetValueText(float value)
    {
        _fillText.text = value.ToString("0");
        _fillText.rectTransform.SetWidth(PercentToPos(_fillValue/MaxValue).x + 0.5f);
    }
    private string GenerateTooltip()
    {
        string res = $"-{gameObject.name.ToUpper()}-\n";

        res += $"<color=#{_fillBar.color.ToHexString()}>current: {_fillValue}</color>\n";
        res += $"<color=#{_lowLimit.color.ToHexString()}>min: {_lowLimitValue}</color>\n";
        res += $"<color=#{_ideal.color.ToHexString()}>ideal: {_idealValue}</color>\n";
        res += $"<color=#{_highLimit.color.ToHexString()}>max: {_highLimitValue}</color>\n";

        return res;
    }
}
