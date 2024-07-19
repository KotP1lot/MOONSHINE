using DG.Tweening;
using System;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public event Action<Ingredient> OnParentChange;
    public SOIngredient Data;

    private GameObject _model;
    private Rigidbody2D _rb;
    private Collider2D _collider;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

        if (Data != null)
        {
            Setup(Data);
        }
    }

    public void Setup(SOIngredient so)
    {
        Data = so;
        // _model = Instantiate(so.Model, transform);
        // _model.transform.localPosition = Vector3.zero;
        GetComponent<SpriteRenderer>().color = so.Color;
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
    public void SetRbType(RigidbodyType2D type) => _rb.bodyType = type;
    public void SetActiveCollider(bool isActive) => _collider.enabled = isActive;
    public void ResetLocalPosition()
    {
        transform.localPosition = Vector2.zero;
        transform.rotation = Quaternion.identity;
    }
}
