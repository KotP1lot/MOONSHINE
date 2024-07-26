using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public event Action<Ingredient> OnParentChange;
    public event Action<Ingredient> OnClick;
    public SOIngredient Data;
    public bool IsUsed;

    [SerializeField] private GameObject _model;
    private Rigidbody _rb;
    private CursorHover _hover;

    private Rigidbody[] _rbs;
    private MeshCollider _collider;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _hover = GetComponentInChildren<CursorHover>();

        _rbs = GetComponentsInChildren<Rigidbody>();
        _collider = GetComponentInChildren<MeshCollider>();

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

        if(_model.transform.childCount>0)
        {
            Destroy(_model.GetComponentsInChildren<MeshRenderer>()[1].gameObject);
        }

        if (so.ChildPrefab != null)
        {
            var obj = Instantiate(so.ChildPrefab);
            obj.transform.SetParent(_model.transform);
            obj.transform.localPosition = Vector3.zero + so.ChildPrefab.transform.position;
            obj.transform.localRotation = Quaternion.identity;

        }

        _hover.SetTooltip(GenerateTooltip(so));
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
    public void ResetLocalPosition()
    {
        transform.localPosition = Vector2.zero;
        transform.rotation = Quaternion.identity;
        _model.transform.rotation = Quaternion.identity;
    }

    public void SetLayer(LayerMask layer)
    {
        gameObject.layer = layer;
        _model.layer = layer;
    }

    public void EnablePhysics(bool enable)
    {
        foreach (var rb in _rbs)
            rb.isKinematic = !enable;
        _collider.enabled = enable;
    }

    private string GenerateTooltip(SOIngredient so)
    {
        string res = $"-{so.name.ToUpper()}-\n";
        string[] names = new string[] { "alcohol", "toxic", "sweet", "bitter", "sour" };

        for(int i =  0; i < names.Length; i++)
        {
            var stat = so.Stats.Array[i];
            string numberColor = stat < 0 ? $"</color><color=#{GameManager.Instance.Colors.NegativeValue.ToHexString()}>" : "";

            if (stat != 0) res += $"<color=#{GameManager.Instance.Colors.Array[i].ToHexString()}>" +
                    $"{names[i]}: " +
                    $"{numberColor}{stat}</color>\n";
        }

        return res;
    }
}
