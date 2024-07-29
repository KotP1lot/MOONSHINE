using DG.Tweening;
using System;
using UnityEngine;

public class ClientMovement : MonoBehaviour
{
    public Action OnClientReady;

    private ClientVisual _visual;

    private void Awake()
    {
        _visual = GetComponent<ClientVisual>();
    }

    public void MoveIn()
    {
        MoveAnim(true, () =>
        {
            transform.DOLocalMoveY(0.2f + _visual.Offset.y, 0.2f);
            transform.DOLocalMoveY(0 + _visual.Offset.y, 0.2f).SetDelay(0.2f);
            OnClientReady?.Invoke();
        });
    }
    public void MoveOut()
    {
        MoveAnim(false, () => {
        });
    }
    public void MoveAnim(bool isEnter, Action func)
    {

        Tween wobbling = transform.DOLocalMoveY(-0.5f + _visual.Offset.y, 0.15f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        transform.DOLocalMoveX((isEnter ? 0 : -15) + _visual.Offset.x, isEnter? 2f:1.8f).SetEase(Ease.Linear).OnComplete(() =>
        {
            wobbling.Kill();
            func();
        });
    }

    public void DieAnim() 
    {
        transform.DOLocalMoveY(-10f, 2f).SetEase(Ease.InOutBack);
    }
    private void OnDisable()
    {
        OnClientReady = null;
    }
}
