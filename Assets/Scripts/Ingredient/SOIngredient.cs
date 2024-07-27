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
    public Vector3 SpawnPos;

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

    [HideInInspector]public int EnhancedStat;

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

    public void EnhanceStat(int index, float percent)
    {
        EnhancedStat = index;
        switch(index)
        {
            case 0: Alcohol += GetRounded(Alcohol * percent); break;
            case 1: Toxicity += GetRounded(Toxicity * percent); break;
            case 2: Sweetness += GetRounded(Sweetness * percent); break;
            case 3: Bitterness += GetRounded(Bitterness * percent); break;
            case 4: Sourness += GetRounded(Sourness * percent); break;
        };
    }

    private float GetRounded(float value)
    {
        if(value < 0) return Mathf.FloorToInt(value);
        return Mathf.CeilToInt(value);
    }
}
