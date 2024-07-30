using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MainMenuScreen : MonoBehaviour
{
    [SerializeField] SpriteRenderer _title;
    [SerializeField] Transform _startLabel;
    
    private Sequence _titleSequence;
    private Sequence _startLabelSequence;
    
    public void Start()
    {
        _titleSequence = DOTween.Sequence()
            .Append(_title.DOFade(0, 1f).SetEase(Ease.OutFlash, 14, 1))
            .AppendInterval(3)
            .Append(_title.DOFade(0, 1f).SetEase(Ease.OutFlash, 10, 1))
            .AppendInterval(4)
            .SetLoops(-1);

        _startLabelSequence = DOTween.Sequence()
            .Append(_startLabel.DOLocalMoveY(-0.1f, 1.5f).SetEase(Ease.InOutFlash, 2, 0))
            .SetLoops(-1);
    }

    public void Stop()
    {
        _titleSequence.Kill();
        _startLabelSequence.Kill();

        _title.DOFade(1, 0);
        _startLabel.DOLocalMoveY(-0.3f, 0);
    }
}
