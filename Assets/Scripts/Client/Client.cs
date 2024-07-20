using System;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    [Serializable]
    public class Stat
    {
        [SerializeField] public float currentValue;
        [SerializeField] private float lowerThreshold;
        [SerializeField] private float upperThreshold;

        public bool IsAboveUpperThreshold() => currentValue > upperThreshold;
        public bool IsBelowLowerThreshold() => currentValue < lowerThreshold;
    }

    protected Stat toxicity;
    protected Stat alcohol;
    protected Stat bitterness;
    protected Stat sweetness;
    protected Stat sourness;

    protected Stat[] allStats;

    protected bool isDead = false;

    public event Action<Client> OnClientDied;
    public event Action<Client> OnClientReady;
    public event Action<Client> OnClientSatisfied;
   
    private ClientMovement _movement;
    private ClientVisual _visual;

    private void Awake()
    {
        allStats = new Stat[] { toxicity, alcohol, bitterness, sweetness, sourness };
        _movement = GetComponent<ClientMovement>();
        _visual = GetComponent<ClientVisual>();
        _movement.OnCustomerReady += () => OnClientReady?.Invoke(this);
        _movement.OnExitAnimFinished += () => OnClientSatisfied?.Invoke(this);
    }
    public void Spawn(List<Sprite> sprites) 
    {
        _visual.Setup(sprites);
        _movement.MoveIn();
    }

    protected virtual void UpdateStats()
    {
        if (isDead) return;

        bool isFeelingSick = false;
        bool allAboveThreshold = true;

        foreach (var stat in allStats)
        {
            if (stat.IsAboveUpperThreshold())
            {
                isFeelingSick = true;
            }
            else
            {
                allAboveThreshold = false;
            }
            if (isFeelingSick && !allAboveThreshold)
            {
                break;
            }
        }

        if (allAboveThreshold)
        {
            Die();
            return;
        }
        if (isFeelingSick)
            FeelSick();
    }

    protected void FeelSick()
    {
        Debug.Log("ploha clientu");
    }

    protected void Die()
    {
        if (!isDead)
        {
            isDead = true;
            OnClientDied?.Invoke(this);
            Debug.Log("client pomer!");
        }
    }
    public void Drink(Stats cock) 
    {
        toxicity.currentValue += cock.Toxicity;
        sweetness.currentValue += cock.Sweetness;
        alcohol.currentValue += cock.Alcohol;
        bitterness.currentValue += cock.Bitterness;
        sourness.currentValue += cock.Sourness;
        UpdateStats();
    }
}

