using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum CursorType { Default, Click, Grab, Drag }
public class CursorController : MonoBehaviour
{
    public static CursorController Instance;

    private Sprite _defaultSprite;
    [SerializeField] private Sprite _clickSprite;
    [SerializeField] private Sprite _grabSprite;
    [SerializeField] private Sprite _dragSprite;

    private Image _image;
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
        _image = GetComponent<Image>();
        _defaultSprite = _image.sprite;
        _screenScale = Screen.width / 320;
    }

    private void Update()
    {
        transform.localPosition = Input.mousePosition / _screenScale - new Vector3(160,90);
    }

    public void SetSprite(CursorType type = CursorType.Default)
    {
        Sprite sprite = _defaultSprite;

        switch(type)
        {
            case CursorType.Click: sprite = _clickSprite; break;
            case CursorType.Grab: sprite = _grabSprite; break;
            case CursorType.Drag: sprite = _dragSprite; break;
        }

        _image.sprite = sprite;
        _image.rectTransform.pivot = sprite.pivot / _image.rectTransform.sizeDelta.x;
    }
}
