using System;
using System.Collections.Generic;
using System.Linq;
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

    public void CopyFrom(SOIngredient other)
    {
        if (other == null) return;

        Mesh = other.Mesh;
        Material = other.Material;
        Stats = other.Stats;
        Rarity = other.Rarity;
        Unlocked = other.Unlocked;
        IsEnhanced = other.IsEnhanced;
    }
}
[Serializable]
public class Stats
{
    public float Alcohol;
    public float Toxicity;
    public float Sweetness;
    public float Bitterness;
    public float Sourness;
    public float Uniqueness;

    public float[] Array
    {
        get
        {
            return new[] { Alcohol, Toxicity, Sweetness, Bitterness,Sourness };
        }
    }

    public int GetHighestStatIndex()
    {
        float max = 0;
        List<int> maxIndexes = new List<int>();

        for(int i = 0; i < Array.Length; i++)
        {

            if (Array[i] > max)
            {
                max= Array[i];

                maxIndexes.Clear();
                maxIndexes.Add(i);
            }
            else if (Array[i] == max)
            {
                maxIndexes.Add(i);
            }
        }

        int rand = UnityEngine.Random.Range(0, maxIndexes.Count);
        return maxIndexes[rand];
    }
}
