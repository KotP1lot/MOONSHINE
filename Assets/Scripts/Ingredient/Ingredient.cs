using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Ingredient : Item
{
    public SOIngredient Data;
    public bool IsUsed;

    private CursorHover _hover;
    private GameObject _modelChild;
    private MeshRenderer _meshRenderer;

    public Transform Child { get { return _model.transform; } }

    private void Awake()
    {
        _hover = GetComponentInChildren<CursorHover>();
        _meshRenderer = _model.GetComponent<MeshRenderer>();
        _modelChild = _model.GetComponentsInChildren<MeshRenderer>()[1].gameObject;

        IsUsed = false;
    }
    public override void Setup(ScriptableObject so, bool bo = true)
    {
        SOIngredient ingredient = so as SOIngredient;
        Data = ingredient;
        _model.GetComponent<MeshFilter>().mesh = ingredient.Mesh;
        _meshRenderer.material = ingredient.Material;
        _model.GetComponent<MeshCollider>().sharedMesh = ingredient.Mesh;

        _modelChild.GetComponent<MeshFilter>().mesh = ingredient.ChildMesh;
        _modelChild.GetComponent<MeshRenderer>().material = ingredient.ChildMaterial;

        _hover.SetTooltip(GenerateTooltip(ingredient));
        if(bo) Utility.Delay(Time.deltaTime, () => Spawn());
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
    public override void Spawn()
    {
        transform.localPosition = Vector3.zero + Data.SpawnPos;
        transform.rotation = Quaternion.identity;
        _model.transform.rotation = Quaternion.identity;

        base.Spawn();
    }
}
