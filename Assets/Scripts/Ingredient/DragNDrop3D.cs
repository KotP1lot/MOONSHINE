using System;
using UnityEngine;

public class DragNDrop3D : MonoBehaviour
{
    public  event Action OnClick;
    private Rigidbody _rb;
    private ConfigurableJoint _joint;
    private Rigidbody _jointRB;
    private Vector3 _offset;
    private MeshCollider _collider;
    private Item _item;

    void Start()
    {
        _item = GetComponentInParent<Item>();
        _rb = GetComponent<Rigidbody>();
        _joint = GetComponentInParent<ConfigurableJoint>();
        _jointRB = _joint.GetComponent<Rigidbody>();
        _collider = GetComponent<MeshCollider>();
    }
    private void OnDestroy()
    {
        OnClick = null;
    }
    void FixedUpdate()
    {
        if (_jointRB == null) return;

        if (_jointRB.velocity.magnitude > 3) _rb.angularDrag = 0.05f;
        else _rb.angularDrag = 5;
    }

    public virtual void OnMouseDown()
    {
        OnClick?.Invoke();
        if (_item.Price != 0) return;

        _joint.transform.ZtoZero();
        transform.ZtoZero(local: true);

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _offset = (transform.parent.position - new Vector3(pos.x, pos.y, transform.parent.position.z)) * -1;
        _joint.anchor = _offset;

        _collider.enabled = false;
        _jointRB.isKinematic = true;
    }
    public virtual void OnMouseDrag()
    {
        if(_item.Price!=0) return;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _jointRB.MovePosition(new Vector3(pos.x,pos.y) -_offset);
    }

    public virtual void OnMouseUp()
    {
        _collider.enabled = true;
        _jointRB.isKinematic = false;

        _offset = Vector3.zero;
        _joint.anchor = Vector3.zero;
    }
}
