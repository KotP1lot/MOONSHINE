using System;
using UnityEngine;
[CreateAssetMenu(fileName = "IngredientVariant", menuName = "Ingredient/IngredientVariant")]
public class SOIngredient : ScriptableObject
{
    [Header("_visual")]
    public Mesh Mesh;
    public Material Material;
    public GameObject ChildPrefab;

    [Header("Stats")]
    public Stats Stats;
    public string Rarity;
    public bool Unlocked;
    public bool IsEnhanced { get; set; }
}
[Serializable]
public class Stats 
{
    public float Toxicity;
    public float Alcohol;
    public float Bitterness;
    public float Sweetness;
    public float Sourness;

    public float[] Array
    {
        get
        {
            return new[] { Alcohol, Toxicity, Sweetness, Bitterness,Sourness };
        }
    }
}
