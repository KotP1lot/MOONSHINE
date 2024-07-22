using System;
using UnityEngine;
[CreateAssetMenu(fileName = "IngredientVariant", menuName = "Ingredient/IngredientVariant")]
public class SOIngredient : ScriptableObject
{
    [Header("_visual")]
    public Mesh Mesh;
    public Material Material;

    [Header("Stats")]
    public Stats Stats;
    public string Rarity;
    public bool Unlocked;
    public bool IsEnhanced { get; set; }
}
[Serializable]
public struct Stats 
{
    public float Alcohol;
    public float Toxicity;
    public float Sweetness;
    public float Bitterness;
    public float Sourness;
    public float Uniqueness;

}
