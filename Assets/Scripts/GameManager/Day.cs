using System;
using UnityEngine;

[Serializable]
public struct Day 
{
    [Header("Queue")]
    public int ClientCount;
    public int PoliceCount;
    public int UnderoverPoliceCount;

    [Header("Bribe")]
    public int MinBribe;
    public int MaxBribe;

    [Header("ClientStat")]
    [Range(0,0.8f)] public float MaxLowerThreshold;
    [Range(0.1f, 0.8f)] public float StepThreshold;
}
