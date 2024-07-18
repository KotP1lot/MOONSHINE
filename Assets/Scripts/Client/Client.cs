using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    [System.Serializable]
    public struct Stat
    {
        [SerializeField] private float currentValue;
        [SerializeField] private float lowerThreshold;
        [SerializeField] private float upperThreshold;

        public bool IsAboveUpperThreshold() => currentValue > upperThreshold;
        public bool IsBelowLowerThreshold() => currentValue < lowerThreshold;
    }

    public Stat toxicity;
    public Stat alcohol;
    public Stat bitterness;
    public Stat sweetness;
    public Stat sourness;

    private Stat[] allStats;

    private bool isDead = false;

    private void Awake()
    {
        allStats = new Stat[] { toxicity, alcohol, bitterness, sweetness, sourness };
    }

    public void UpdateStats()
    {
        if (isDead) return;

        bool isFeelingSick = false;

        foreach (var stat in allStats)
        {
            if (stat.IsAboveUpperThreshold())
            {
                isFeelingSick = true;
                break;
            }
        }

        if (isFeelingSick)
        {
            FeelSick();
        }

        if (AreAllStatsAboveUpperThreshold())
        {
            Die();
        }
    }

    private bool AreAllStatsAboveUpperThreshold()
    {
        foreach (var stat in allStats)
        {
            if (!stat.IsAboveUpperThreshold())
            {
                return false;
            }
        }
        return true;
    }

    private void FeelSick()
    {
        Debug.Log("ploha clientu");
    }

    private void Die()
    {
        if (!isDead)
        {
            isDead = true;
            Debug.Log("client pomer!");
        }
    }
}

