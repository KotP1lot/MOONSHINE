using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    protected Rigidbody[] _rbs;
    protected MeshCollider _collider;

    private void Awake()
    {
        _rbs = GetComponentsInChildren<Rigidbody>();
        _collider = GetComponentInChildren<MeshCollider>();
    }

    public virtual void EnablePhysics(bool enable)
    {
        if (_rbs is null) Awake();

        foreach (var rb in _rbs)
            rb.isKinematic = !enable;
        _collider.enabled = enable;
    }

    public void ResetChildPosition(float duration)
    {
        _collider.transform.DOLocalMove(Vector3.zero, duration);
    }
}
