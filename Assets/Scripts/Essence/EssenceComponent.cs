using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EssenceComponent : Item
{
    private StatType _type;
    private float _strength;
    private Essence _essence;

    private MeshRenderer _renderer;
    [HideInInspector]public ParticleSystem Particles;

    public void SetEssence(Essence essence)
    {
        _essence = essence;
        _type = essence.Type;
        _strength = essence.Strength;

        var color = GameManager.Instance.Colors.Array[(int)essence.Type];

        Particles = GetComponentInChildren<ParticleSystem>();
        var main = Particles.main;
        main.startColor = color;

        _renderer = GetComponentInChildren<MeshRenderer>();
        var mat = Instantiate(_renderer.material);
        mat.color = color;
        _renderer.material = mat;

        GetComponentInChildren<CursorHover>().SetTooltip(GenerateTooltip());
    }

    public Essence Essence => _essence;
    public StatType Type => _type;
    public float Strength => _strength;

    public override void EnablePhysics(bool enable)
    {
        base.EnablePhysics(enable);
        if (enable) Particles.Play();
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
