using System;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public event Action<Ingredient> OnParentChange;
    public event Action<Ingredient> OnClick;
    public SOIngredient Data;
    public bool IsUsed;

    [SerializeField] private GameObject _model;
    private Rigidbody _rb;
    private Collider2D _collider;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider2D>();
        IsUsed = false;
        GetComponentInChildren<DragNDrop3D>().OnClick += () => OnClick?.Invoke(this);
        if (Data != null)
        {
            Setup(Data);
        }
    }

    public void Setup(SOIngredient so)
    {
        Data = so;
        _model.GetComponent<MeshFilter>().mesh = so.Mesh;
        _model.GetComponent<MeshRenderer>().material = so.Material;
        _model.GetComponent<MeshCollider>().sharedMesh = so.Mesh;
    }

    public Stats GetStats()
    {
        return Data.Stats;
    }
    public void SetParent(Transform parent)
    {
        transform.parent = parent;
        OnParentChange?.Invoke(this);
    }
    public void SetKinematic(bool isKinematic) => _rb.isKinematic = isKinematic;
    public void SetActiveCollider(bool isActive) => _collider.enabled = isActive;
    public void ResetLocalPosition()
    {
        transform.localPosition = Vector2.zero;
        transform.rotation = Quaternion.identity;
    }

    public void SetLayer(LayerMask layer)
    {
        gameObject.layer = layer;
        _model.layer = layer;
    } 
}
