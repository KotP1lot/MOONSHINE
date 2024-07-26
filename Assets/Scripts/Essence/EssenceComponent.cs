using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EssenceComponent : MonoBehaviour
{
    [SerializeField] private Essence.EssenceType _type;
    [SerializeField] private float _strength;
    [SerializeField] private float _enhancementPercentage;

    private Essence _essence;
    private Rigidbody[] _rbs;
    private MeshCollider _collider;
    private void Start()
    {
        _rbs = GetComponentsInChildren<Rigidbody>();
        _collider = GetComponentInChildren<MeshCollider>();
    }

    // Метод для установки эссенции
    public void SetEssence(Essence essence)
    {
        _essence = essence;
        _type = essence.Type;
        _strength = essence.Strength;
        _enhancementPercentage = CalculateEnhancementPercentage(_strength);

        GetComponentInChildren<CursorHover>().SetTooltip(GenerateTooltip());
    }

    // Метод для получения эссенции
    public Essence GetEssence() => _essence;

    // Метод для вычисления процента увеличения на основе силы
    private float CalculateEnhancementPercentage(float strength)
    {
        if (strength <= 25) return 0.1f;
        if (strength <= 50) return 0.2f;
        if (strength <= 75) return 0.3f;
        return 0.5f;
    }

    // Методы для доступа к полям
    public Essence.EssenceType Type => _type;
    public float Strength => _strength;
    public float EnhancementPercentage => _enhancementPercentage;

    public void EnablePhysics(bool enable)
    {
        foreach(var rb in _rbs)
            rb.isKinematic = !enable;
        _collider.enabled = enable;
    }

    private string GenerateTooltip()
    {
        string res = $"-{_essence.Type.ToString().Replace("ness", "").Replace("ity", "").ToUpper()} ESSENCE-\n";

        res += $"<color=#{GameManager.Instance.Colors.Array[(int)_essence.Type].ToHexString()}>" +
            $"{_essence.Type.ToString().ToLower()} " +
            $"+{(_strength * 100).ToString("0")}%";

        return res;
    }

}
