using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AparatChanger : MonoBehaviour
{
    [SerializeField] private Aparat _barrel;
    [SerializeField] private Aparat _combinator;
    [SerializeField] private Aparat _centrifuge;

    [Space(10)]
    [SerializeField] private float _xCoordinate;
    [SerializeField] private float _appearDelay;
    [SerializeField] private Ease _hideEase = Ease.InBack;
    [SerializeField] private Ease _appearEase = Ease.OutBack;


    private TweenCallback _callback;
    private bool _isChecking;
    private Button[] _buttons;

    private void Start()
    {
        _buttons = GetComponentsInChildren<Button>();

        _barrel.DefaultX = _barrel.transform.position.x;
        _combinator.DefaultX = _combinator.transform.position.x;
        _centrifuge.DefaultX = _centrifuge.transform.position.x;

        if (!_barrel.gameObject.activeSelf) { MoveToStack(_barrel.transform, 0); _barrel.ChangeState(() => { }); };
        if (!_combinator.gameObject.activeSelf) MoveToStack(_combinator.transform,0);
        if (!_centrifuge.gameObject.activeSelf) MoveToStack(_centrifuge.transform,0);
    }

    private void Update()
    {
        if(_isChecking)
        {
            if(!_barrel.gameObject.activeSelf
                && !_centrifuge.gameObject.activeSelf
                && !_combinator.gameObject.activeSelf)
            {
                _isChecking = false;
                _callback?.Invoke();
            }
        }
    }

    public void ChangeToBarrel()
    {
        if (_barrel.gameObject.activeSelf) return;
        if (_isChecking) return;
        Hide(new Aparat[] { _centrifuge, _combinator });
        _callback = () => { Appear(_barrel); };
    }
    public void ChangeToCentrifuge()
    {
        if (_centrifuge.gameObject.activeSelf) return;
        if (_isChecking) return;
        Hide(new Aparat[] { _barrel, _combinator });
        _callback = () => { Appear(_centrifuge); };
    }
    public void ChangeToCombinator()
    {
        if (_combinator.gameObject.activeSelf) return;
        if (_isChecking) return;
        Hide(new Aparat[] { _barrel, _centrifuge });
        _callback = () => { Appear(_combinator); };

    }

    private void MoveToStack(Transform transform, float duration = 0.5f)
    {
        transform.DOMoveX(_xCoordinate, duration).SetEase(_hideEase)
            .onComplete = () => transform.gameObject.SetActive(false);
    }

    private void Hide(Aparat[] aparats)
    {
        _isChecking = true;

        foreach(var aparat in aparats)
        {
            if(aparat.gameObject.activeSelf)
            {
                aparat.ChangeState(() => { MoveToStack(aparat.transform); });
            }
        }
    }

    private void Appear(Aparat aparat)
    {
        aparat.transform.gameObject.SetActive(true);
        aparat.transform.DOMoveX(aparat.DefaultX, 0.5f).SetEase(_appearEase).SetDelay(_appearDelay)
            .onComplete = ()=> { aparat.ChangeState(() => { }); };
    }

    public void EnableButtons(bool enable)
    {
        foreach (var button in _buttons)
            button.interactable = enable;
    }
}
