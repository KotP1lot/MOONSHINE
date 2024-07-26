using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorCanvas : MonoBehaviour
{
    private TextMeshProUGUI[] _texts;
    private CanvasGroup _canvasGroup;

    private Tweener _flash;

    void Start()
    {
        _texts = GetComponentsInChildren<TextMeshProUGUI>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ShowText(string text, float duration = 3)
    {
        _flash = _canvasGroup.DOFade(1, 0.3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutCirc);

        foreach(var t in _texts)
            t.text = text.ToUpper();

        Utility.Delay(duration, () => { _flash.Kill(); _canvasGroup.alpha = 0; });
    }
}
