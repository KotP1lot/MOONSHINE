using System;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    [Serializable]
    public class Stat
    {
        [SerializeField] public float CurrentValue;
        [SerializeField] public float LowerThreshold;
        [SerializeField] public float UpperThreshold;

        public bool IsAboveUpperThreshold() => CurrentValue > UpperThreshold;
        public bool IsBelowLowerThreshold() => CurrentValue < LowerThreshold;

        public void SetStat() 
        {
            LowerThreshold = UnityEngine.Random.Range(0, 80);
            UpperThreshold = UnityEngine.Random.Range((int)LowerThreshold+10, 100);
        }
    }

    protected Stat _toxicity = new();
    protected Stat _alcohol = new();
    protected Stat _bitterness = new();
    protected Stat _sweetness = new();
    protected Stat _sourness = new();

    public List<Stat> AllStats;

    protected bool _isDead = false;

    public bool isReady;

    public event Action<Client> OnClientDied;
    public event Action<Client> OnClientReady;
    public event Action<Client> OnClientSatisfied;
   
    protected ClientMovement _movement;
    protected ClientVisual _visual;

    private void Awake()
    {
        AllStats = new () { _toxicity, _alcohol, _bitterness, _sweetness, _sourness };
        _movement = GetComponent<ClientMovement>();
        _visual = GetComponent<ClientVisual>();
        _movement.OnCustomerReady += () => OnClientReady?.Invoke(this);
        _movement.OnExitAnimFinished += () => OnClientSatisfied?.Invoke(this);
    }
    public void Spawn(SOClient client) 
    {
        SetStat();
        transform.position = new Vector2(15, 0);
        _visual.Setup(GetSprites(client));
        _movement.MoveIn();
    }
    protected List<Sprite> CollectSprites(SOClient client, List<Accessory> accessories)
    {
        List<Sprite> sprites = new() { client.BaseSprite };
        AccessoryType[] accessoryTypes = (AccessoryType[])Enum.GetValues(typeof(AccessoryType));
        foreach (AccessoryType type in accessoryTypes)
        {
            List<Accessory> filteredAccessories = accessories.FindAll(x => x.Type == type);
            if (filteredAccessories.Count == 0) continue;
            int random = UnityEngine.Random.Range(0, filteredAccessories.Count + 1);
            if (random == filteredAccessories.Count) continue;
            sprites.Add(filteredAccessories[random].Sprite);
        }
        return sprites;
    }

    protected virtual List<Sprite> GetSprites(SOClient client)
    {
        return CollectSprites(client, client.Accessories);
    }

    protected void SetStat() 
    {
        AllStats.ForEach(x => x.SetStat());
        Debug.Log($"{AllStats[0].LowerThreshold} : {AllStats[0].UpperThreshold}");
    }
    protected virtual void UpdateStats()
    {
        if (_isDead) return;

        bool isFeelingSick = false;
        bool allAboveThreshold = true;

        foreach (var stat in AllStats)
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
        _movement.MoveOut();
    }

    protected void FeelSick()
    {
        Debug.Log("ploha clientu");
    }

    protected void Die()
    {
        if (!_isDead)
        {
            _isDead = true;
            OnClientDied?.Invoke(this);
            Debug.Log("client pomer!");
        }
    }
    public void Drink(Stats cock) 
    {
        _toxicity.CurrentValue += cock.Toxicity;
        _sweetness.CurrentValue += cock.Sweetness;
        _alcohol.CurrentValue += cock.Alcohol;
        _bitterness.CurrentValue += cock.Bitterness;
        _sourness.CurrentValue += cock.Sourness;
        UpdateStats();
    }
}

