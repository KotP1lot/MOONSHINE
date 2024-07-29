using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AparatChanger : MonoBehaviour
{
    [SerializeField] private Aparat[] _aparats;
    [SerializeField] private Collider _janitor;

    [Space(10)]
    [SerializeField] private float _xCoordinate;
    [SerializeField] private float _appearDelay;
    [SerializeField] private Ease _hideEase = Ease.InBack;
    [SerializeField] private Ease _appearEase = Ease.OutBack;

    private TweenCallback _callback;
    private bool _isChecking;
    private bool _IsMoving;
    private Button[] _buttons;
    private List<RectTransform> _buttonRects = new List<RectTransform>();
    private float _buttonX;

    private void Start()
    {
        _buttons = GetComponentsInChildren<Button>();
        foreach(var button in _buttons)
            _buttonRects.Add(button.GetComponent<RectTransform>());
        _buttonX = _buttonRects[0].anchoredPosition.x;

        for (int i = 0; i < _aparats.Length; i++) 
        {
            _aparats[i].DefaultX = _aparats[i].transform.position.x;
            if (i!=0) { MoveToStack(_aparats[i].transform, 0); _aparats[i].ChangeState(() => { }); };
            if (_aparats[i].TryGetComponent(out IUpgrade component))
            {
                component.OnUnlock += EnableButton;
            }
        }
        EnableButton(false, 1);
        EnableButton(false, 2);

        SetActiveButton(0);
    }

    private void Update()
    {
        if(_isChecking)
        {
            if(_aparats.Count(x=>x.gameObject.activeSelf)==0)
            {
                _isChecking = false;
                _callback?.Invoke();
            }
        }
    }

    public void ChangeToIndex(int index)
    {
        if (_aparats[index].gameObject.activeSelf) return;
        if (_isChecking || _IsMoving) return;
        Hide(_aparats.Where(x=>Array.IndexOf(_aparats,x)!=index).ToArray());
        _callback = () => { Appear(_aparats[index]); };
        AudioManager.instance.Play("Swap");

        SetActiveButton(index);
    }

    private void MoveToStack(Transform transform, float duration = 0.5f)
    {
        transform.DOMoveX(_xCoordinate, duration).SetEase(_hideEase)
            .onComplete = () => transform.gameObject.SetActive(false);
    }

    private void Hide(Aparat[] aparats)
    {
        _isChecking = true;
        _IsMoving = true;

        _janitor.enabled = true;
        Utility.Delay(0.1f, ()=>_janitor.enabled = false);

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
            .onComplete = ()=> { aparat.ChangeState(() => { _IsMoving = false; }); };
    }
    public void EnableButton(bool enable, int id) 
    {
        _buttons[id].gameObject.SetActive(enable);
    }
    public void EnableButtons(bool enable)
    {
        foreach (var button in _buttons)
            button.interactable = enable;
    }

    public void SetActiveButton(int index)
    {
        foreach (var button in _buttonRects)
            button.DOAnchorPosX(_buttonX,0.2f).SetEase(Ease.OutCirc);

        _buttonRects[index].DOAnchorPosX(_buttonX-4, 0.3f).SetEase(Ease.OutCirc);
    }
}
