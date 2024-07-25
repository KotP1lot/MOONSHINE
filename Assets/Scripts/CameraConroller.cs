using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum CameraPosType 
{
    Client,
    Brewery,
    Loots
}
public class CameraConroller : MonoBehaviour
{
    [SerializeField] Image _fade;
    private void Start()
    {
        GlobalEvents.Instance.OnChangeCameraPos += Rotate;
    }
    private void OnDisable()
    {
        GlobalEvents.Instance.OnChangeCameraPos -= Rotate;
    }
    public void Rotate(CameraPosType type)
    {
        float posY = type switch
        {
            CameraPosType.Client => 180,
            CameraPosType.Brewery => 0,
            CameraPosType.Loots => 90,
            _ => 180
        };
        _fade.DOFade(1, 0.2f);
        transform.DORotate(new Vector3(0, posY), 0f).SetDelay(0.2f).
            OnComplete(
            () =>
            {
                _fade.DOFade(0, 0.2f);
            }
            );
    }
}
