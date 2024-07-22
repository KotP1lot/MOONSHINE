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

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _joint = GetComponentInParent<ConfigurableJoint>();
        _jointRB = _joint.GetComponent<Rigidbody>();
        _collider = GetComponent<MeshCollider>();
    }

    void FixedUpdate()
    {
        if (_jointRB.velocity.magnitude > 3) _rb.angularDrag = 0.05f;
        else _rb.angularDrag = 5;
    }

    public virtual void OnMouseDown()
    {
        _joint.transform.ZtoZero();
        transform.ZtoZero(local: true);

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _offset = (transform.parent.position - new Vector3(pos.x, pos.y, transform.parent.position.z)) * -1;
        _joint.anchor = _offset;

        _collider.enabled = false;
        _jointRB.isKinematic = true;
        OnClick?.Invoke();
    }
    public virtual void OnMouseDrag()
    {
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
