using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class WaterSpring : MonoBehaviour
{
    private float _velocity;
    private float _targetHeight = 0;
    private int _index;
    private float _resistance;
    private WaterShapeController _shapeController;
    public float Height { get { return transform.localPosition.y; } }

    public void UpdateSpring(float stiffness, float dampening)
    {
        var height = transform.localPosition.y;

        var offset = height - _targetHeight;
        var loss = -dampening * _velocity;

        _velocity += -stiffness * offset + loss;
        
        var y = transform.localPosition.y + _velocity;
        transform.localPosition = new Vector3(transform.localPosition.x, y , transform.localPosition.z);

        WavePointUpdate();
    }

    private void WavePointUpdate()
    {
        if (_shapeController is null) return;

        Vector3 point = _shapeController.Spline.GetPosition(_index);
        _shapeController.Spline.SetPosition(_index, new Vector3(point.x, transform.localPosition.y, point.z));
    }

    public void Init(int index,WaterShapeController shapeController,float resistance)
    {
        _index = index;
        _shapeController = shapeController;
        _resistance = resistance;

        _velocity = 0;
        _targetHeight = transform.localPosition.y;
    }

    public void AddVelocity(float velocity)
    {
        _velocity += velocity; 
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("InWater")) return;
        var rb = collision.GetComponent<Rigidbody>();
        var velocity = Mathf.Clamp(rb.velocity.y, -100, -2f);
        AddVelocity(velocity * _resistance);
        collision.gameObject.layer = LayerMask.NameToLayer("InWater");
        rb.useGravity = false;
        _shapeController.Floaters.Add(rb);
        //rb.drag = 3;
        //rb.angularDrag = 3;
        //var parentRB = collision.transform.parent.GetComponent<Rigidbody>();
        //parentRB.useGravity = false;
        //parentRB.drag = 3;
        //parentRB.angularDrag = 3;
        //GetComponent<Collider>().enabled = false;
    }
}
