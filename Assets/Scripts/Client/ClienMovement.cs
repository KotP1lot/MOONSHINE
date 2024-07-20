using DG.Tweening;
using System;
using UnityEngine;

public class ClientMovement : MonoBehaviour
{
    public Action OnExitAnimFinished;
    public Action OnCustomerReady;

    public void MoveIn()
    {
        MoveAnim(true, () =>
        {
            transform.DOLocalMoveY(0, 0.2f);
            OnCustomerReady?.Invoke();
            MoveOut();
        });
    }
    public void MoveOut()
    {
        MoveAnim(false, () => {
            //foreach (var img in accessory)
            //{
            //    Destroy(img.gameObject);
            //}
            OnExitAnimFinished?.Invoke();
        });
    }
    public void MoveAnim(bool isEnter, Action func)
    {
        Tween wobbling = transform.DOLocalMoveY(1, 0.15f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        transform.DOLocalMoveX(isEnter ? 0 : -15, 2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            wobbling.Kill();
            func();
        });
    }
}
