using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class WaterShapeController : MonoBehaviour
{
    [SerializeField] private int _pointCount;
    [Space(10)]
    [SerializeField] private float _stiffness;
    [SerializeField] private float _dampening;
    [SerializeField] private float _waveSpread;
    [SerializeField] private float _resistance;
    [SerializeField] private float _floating;

    private SpriteShapeController _shapeController;
    private List<WaterSpring> _springs = new List<WaterSpring>();
    private List<Rigidbody> _floaters = new List<Rigidbody>();
    public Spline Spline { get { return _shapeController.spline; } }

    private void Start()
    {
        _shapeController = GetComponent<SpriteShapeController>();
        SetPoints();
        CreateSprings();
    }

    private void FixedUpdate()
    {
        foreach (var spring in _springs)
            spring.UpdateSpring(_stiffness,_dampening);
        WaveEffect();

        var surface = _shapeController.spline.GetPosition(1).y;
        foreach (var floater in _floaters)
            floater.AddForce(Vector3.up * (surface - floater.transform.position.y) * _floating);
    }

    private void WaveEffect()
    {
        float[] leftDeltas = new float[_springs.Count];
        float[] rightDeltas = new float[_springs.Count];

        for(int i = 0; i< _springs.Count; i++)
        {
            if(i>0)
            {
                leftDeltas[i] = _waveSpread * (_springs[i].Height - _springs[i-1].Height);
                _springs[i - 1].AddVelocity(leftDeltas[i]);
            }
            if(i<_springs.Count-1)
            {
                rightDeltas[i] = _waveSpread * (_springs[i].Height - _springs[i+1].Height);
                _springs[i + 1].AddVelocity(rightDeltas[i]);
            }
        }
    }

    private void SetPoints()
    {
        if (_shapeController is null) return;

        int pointCount = Spline.GetPointCount();
        int cornerCount = 2;

        for(int i = cornerCount; i<pointCount - cornerCount; i++)
            Spline.RemovePointAt(i);

        Vector3 leftCorner = Spline.GetPosition(1);
        Vector3 rightCorner = Spline.GetPosition(2);

        float width = rightCorner.x-leftCorner.x;
        float spacing = width / (_pointCount + 1);

        for(int i = _pointCount; i > 0; i--)
        {
            float xPos = i * spacing + leftCorner.x;
            Vector3 point = new Vector3(xPos,leftCorner.y,leftCorner.z);
            Spline.InsertPointAt(cornerCount, point);
            Spline.SetHeight(cornerCount, 0.1f);
            Spline.SetCorner(cornerCount, false);
            Smoothen(Spline, cornerCount);
        }
    }
    private void CreateSprings()
    {
        _springs.Clear();
        for (int i = 0; i < _pointCount+2; i++)
        {
            int index = i + 1;
            
            _springs.Add(CreateSpringObject(index));
        }
    }
    private WaterSpring CreateSpringObject(int index)
    {
        GameObject obj = new GameObject("point " + index);
        obj.transform.SetParent(transform, false);
        obj.transform.localPosition = Spline.GetPosition(index) - new Vector3(0,0,transform.position.z);
        obj.transform.localScale = Vector3.one * 0.8f;
        Collider collider = obj.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        WaterSpring spring = obj.AddComponent<WaterSpring>();
        spring.Init(index, this, _resistance);
        return spring;
    }
    private void Smoothen(Spline waterSpline, int index)
    {
        Vector3 position = waterSpline.GetPosition(index);
        Vector3 positionPrev = position;
        Vector3 positionNext = position;
        if (index > 1)
        {
            positionPrev = waterSpline.GetPosition(index - 1);
        }
        if (index - 1 <= _pointCount)
        {
            positionNext = waterSpline.GetPosition(index + 1);
        }

        Vector3 forward = gameObject.transform.forward;

        float scale = Mathf.Min((positionNext - position).magnitude, (positionPrev - position).magnitude) * 0.33f;

        Vector3 leftTangent = (positionPrev - position).normalized * scale;
        Vector3 rightTangent = (positionNext - position).normalized * scale;

        SplineUtility.CalculateTangents(position, positionPrev, positionNext, forward, scale, out rightTangent, out leftTangent);

        waterSpline.SetTangentMode(index,ShapeTangentMode.Continuous);
        waterSpline.SetLeftTangent(index, leftTangent);
        waterSpline.SetRightTangent(index, rightTangent); 
    }

    public void AddFloater(Rigidbody rb)
    {

        rb.gameObject.layer = LayerMask.NameToLayer("InWater");
        rb.useGravity = false;
        _floaters.Add(rb);
    }
}
