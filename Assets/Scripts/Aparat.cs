using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aparat : MonoBehaviour
{
    public float DefaultX;

    public virtual void ChangeState(TweenCallback onComplete)
    {
        onComplete?.Invoke();
    }

}
