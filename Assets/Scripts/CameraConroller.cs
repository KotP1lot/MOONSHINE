using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CameraConroller : MonoBehaviour
{
    [SerializeField] Image _fade;
    private bool _isLookAtClient = true;
    public void Rotate()
    {
        _fade.DOFade(1, 0.2f);
        transform.DORotate(new Vector3(0, _isLookAtClient ? 0 : 180), 0f).SetDelay(0.2f).
            OnComplete(
            () =>
            {
                _fade.DOFade(0, 0.2f);
            }
            );
        _isLookAtClient = !_isLookAtClient;
    }
}
