using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum CursorType { Default, Click, Grab, Drag, Buy }
public class CursorController : MonoBehaviour
{
    public static CursorController Instance;

    private Sprite _defaultSprite;
    [SerializeField] private Sprite _clickSprite;
    [SerializeField] private Sprite _grabSprite;
    [SerializeField] private Sprite _dragSprite;
    [SerializeField] private Sprite _buySprite;

    private RectTransform _rectTransform;
    private Image _image;
    private TooltipController _tooltip;
    private float _screenScale;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else Instance = this;
    }

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponentInChildren<Image>();
        _tooltip = GetComponentInChildren<TooltipController>();
        _defaultSprite = _image.sprite;
        _screenScale = Screen.width / 320;
    }

    private void Update()
    {
        _rectTransform.anchoredPosition = new Vector2(Mathf.Round(Input.mousePosition.x / _screenScale), Mathf.Round(Input.mousePosition.y / _screenScale));
        _tooltip.AdjustPosition(_rectTransform);
    }

    public void SetSprite(CursorType type = CursorType.Default)
    {
        Sprite sprite = _defaultSprite;

        switch(type)
        {
            case CursorType.Click: sprite = _clickSprite; break;
            case CursorType.Grab: sprite = _grabSprite; break;
            case CursorType.Drag: sprite = _dragSprite; break;
            case CursorType.Buy: sprite = _buySprite; break;
        }

        _image.sprite = sprite;
        _image.rectTransform.pivot = sprite.pivot / _image.rectTransform.sizeDelta.x;
    }

    
}
