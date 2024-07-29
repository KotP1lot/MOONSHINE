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
        wobbling.onStepComplete = () => { if(transform.localPosition.y - _visual.Offset.y < -0.4f) AudioManager.instance.Step(); };
        transform.DOLocalMoveX((isEnter ? 0 : -15) + _visual.Offset.x, isEnter? 2f:1.8f).SetEase(Ease.Linear).OnComplete(() =>
        {
            wobbling.Kill();
            func();
        });
    }

    public void DieAnim() 
    {
        transform.DOLocalRotate(new Vector3(0, -270, 60), 2,RotateMode.FastBeyond360).SetEase(Ease.OutQuad);
        transform.DOLocalMoveY(-10f, 2).SetEase(Ease.InQuad);
    }
    private void OnDisable()
    {
        OnClientReady = null;
    }
}
