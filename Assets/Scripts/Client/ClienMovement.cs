using DG.Tweening;
using System;
using UnityEngine;

public class ClientMovement : MonoBehaviour
{
    public Action OnClientReady;

    public void MoveIn()
    {
        MoveAnim(true, () =>
        {
            transform.DOLocalMoveY(0, 0.2f);
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
        Tween wobbling = transform.DOLocalMoveY(1, 0.15f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        transform.DOLocalMoveX(isEnter ? 0 : -15, isEnter? 2f:1.8f).SetEase(Ease.Linear).OnComplete(() =>
        {
            wobbling.Kill();
            func();
        });
    }
}
