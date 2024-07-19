using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragNDrop3D : MonoBehaviour
{
    public bool IsInHand { get; private set; }
    private Rigidbody _rb;
    private ConfigurableJoint _joint;
    private Rigidbody _jointRB;
    private Vector3 _offset;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _joint = GetComponentInParent<ConfigurableJoint>();
        _jointRB = _joint.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (_jointRB.velocity.magnitude > 3) _rb.angularDrag = 0.05f;
        else _rb.angularDrag = 5;
    }

    public virtual void OnMouseDown()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _offset = (transform.parent.position - new Vector3(pos.x, pos.y)) * -1;
        _joint.anchor = _offset;

        IsInHand = true;
        _jointRB.isKinematic = true;
    }
    public virtual void OnMouseDrag()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _jointRB.MovePosition(new Vector3(pos.x,pos.y) -_offset);
    }

    public virtual void OnMouseUp()
    {
        IsInHand = false;
        _jointRB.isKinematic = false;

        _offset = Vector3.zero;
        _joint.anchor = Vector3.zero;
    }



}
