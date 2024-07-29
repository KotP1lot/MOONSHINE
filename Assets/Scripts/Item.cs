using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public event Action<Item> OnParentChange;
    public event Action<Item> OnClick;
    protected Rigidbody[] _rbs;
    protected MeshCollider _collider;
    [SerializeField] protected GameObject _model;
    protected CursorHover _hover;

    public int Price {  get; protected set; }

    private void Awake()
    {
        _rbs = GetComponentsInChildren<Rigidbody>();
        _collider = GetComponentInChildren<MeshCollider>();
        GetComponentInChildren<DragNDrop3D>().OnClick += () => OnClick?.Invoke(this);
    }

    public virtual void EnablePhysics(bool enable)
    {
        if (_rbs is null) Awake();

        foreach (var rb in _rbs)
            rb.isKinematic = !enable;
        _collider.enabled = enable;
    }
    public virtual void Setup(ScriptableObject so, bool bo = true) { }
    public void ResetChildPosition(float duration)
    {
        _collider.transform.DOLocalMove(Vector3.zero, duration);
    }
    public virtual void Spawn()
    { 
        EnablePhysics(false);
        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.2f).SetEase(Ease.OutElastic).onComplete = () => EnablePhysics(true);
    }
    public void SetParent(Transform parent)
    {
        transform.parent = parent;
        OnParentChange?.Invoke(this);
    }
    public virtual void SetLayer(LayerMask layer)
    {
        gameObject.layer = layer;
        _model.layer = layer;
    }
    public void SetPrice(int price)
    {
        if(_hover == null) _hover = GetComponentInChildren<CursorHover>();

        Price = price;
        _hover.SetPrice(PriceType.Silver,price);

        if(price==0) _hover.SetCursor(CursorType.Grab, CursorType.Drag);
        else _hover.SetCursor(CursorType.Buy, CursorType.Buy);
    }
}
