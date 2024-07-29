using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class CustomButton : MonoBehaviour
{
    public bool Animated;
    public UnityEvent OnClick;
    

    public void OnMouseDown()
    {
        OnClick?.Invoke();

        if(Animated) Animate();
    }

    private void Animate()
    {
        transform.localScale = Vector3.one * 0.3f;
        transform.DOScale(1, 0.2f).SetEase(Ease.OutBack);
    }
}
