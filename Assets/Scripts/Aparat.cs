using DG.Tweening;
using UnityEngine;

public class Aparat : MonoBehaviour
{
    [HideInInspector]public float DefaultX;

    public virtual void ChangeState(TweenCallback onComplete)
    {
        onComplete?.Invoke();
    }

}
