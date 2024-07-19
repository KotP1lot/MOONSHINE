using System;
using UnityEngine;
[CreateAssetMenu()]
public class SOIngredient : ScriptableObject
{
    [Header("Visual")]
    public GameObject Model;
    public Color Color;

    [Header("Stats")]
    public Stats Stats;
    public string rarity;
    public bool unlocked;
}
[Serializable]
public struct Stats 
{
    public float Alcohol;
    public float Toxicity;
    public float Sweetness;
    public float Bitterness;
    public float Sourness;
}
