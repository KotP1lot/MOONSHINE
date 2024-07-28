using DG.Tweening;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Ingredient : Item
{
    public event Action<Ingredient> OnParentChange;
    public event Action<Ingredient> OnClick;
    public SOIngredient Data;
    public bool IsUsed;

    [SerializeField] private GameObject _model;
    private Rigidbody _rb;
    private CursorHover _hover;
    private GameObject _modelChild;
    private MeshRenderer _meshRenderer;

    public Transform Child { get { return _model.transform; } }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _hover = GetComponentInChildren<CursorHover>();
        _meshRenderer = _model.GetComponent<MeshRenderer>();
        _modelChild = _model.GetComponentsInChildren<MeshRenderer>()[1].gameObject;

        IsUsed = false;
        GetComponentInChildren<DragNDrop3D>().OnClick += () => OnClick?.Invoke(this);
    }

    public void Setup(SOIngredient so, bool spawn = true)
    {
        Data = so;
        _model.GetComponent<MeshFilter>().mesh = so.Mesh;
        _meshRenderer.material = so.Material;
        _model.GetComponent<MeshCollider>().sharedMesh = so.Mesh;

        _modelChild.GetComponent<MeshFilter>().mesh = so.ChildMesh;
        _modelChild.GetComponent<MeshRenderer>().material = so.ChildMaterial;

        //if (so.ChildMesh != null)
        //{
        //    obj.transform.SetParent(_model.transform);
        //    obj.transform.localPosition = Vector3.zero + so.ChildPrefab.transform.position;
        //    obj.transform.localRotation = Quaternion.identity;


        //}

        _hover.SetTooltip(GenerateTooltip(so));
        if(spawn)Utility.Delay(Time.deltaTime,()=>Spawn());
    }

    public void AddMaterial(Material material)
    {
        var materials = _meshRenderer.materials.ToList();
        materials.Add(material);
        _meshRenderer.SetMaterials(materials);
    }

    public void ApplyEssence(Essence essence)
    {
        SOIngredient enhancedIngredient = Instantiate(Data);
        enhancedIngredient.IsEnhanced = true;
        enhancedIngredient.name = Data.name;

        enhancedIngredient.Stats.EnhanceStat((int)essence.Type, essence.Strength);
        Data = enhancedIngredient;

        _hover.SetTooltip(GenerateTooltip(Data));
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
    public void Spawn()
    {
        transform.localPosition = Vector3.zero + Data.SpawnPos;
        transform.rotation = Quaternion.identity;
        _model.transform.rotation = Quaternion.identity;

        EnablePhysics(false);
        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.2f).SetEase(Ease.OutElastic).onComplete = () => EnablePhysics(true);
    }

    public void SetLayer(LayerMask layer)
    {
        gameObject.layer = layer;
        _model.layer = layer;
    }

    public string GenerateTooltip(SOIngredient so)
    {
        string marker = so.IsEnhanced ? "+" : "-";
        string res = $"{marker}{so.name.ToUpper()}{marker}\n";
        string[] names = new string[] { "alcohol", "toxic", "sweet", "bitter", "sour" };

        for(int i =  0; i < names.Length; i++)
        {
            var stat = so.Stats.Array[i];
            string numberColor = stat < 0 ? $"</color><color=#{GameManager.Instance.Colors.NegativeValue.ToHexString()}>" : "";

            string plus = so.IsEnhanced && so.Stats.EnhancedStat == i ? "+" : "";

            if (stat != 0) res += $"<color=#{GameManager.Instance.Colors.Array[i].ToHexString()}>" +
                    $"{plus}{names[i]}: " +
                    $"{numberColor}{stat}</color>\n";
        }

        return res;
    }
}
