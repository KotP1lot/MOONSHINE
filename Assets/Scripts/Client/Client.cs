using System;
using System.Collections.Generic;
using UnityEngine;

public enum GradeType
{
    S,
    A,
    B,
    C,
    D
}

public class Client : MonoBehaviour
{
    [Serializable]
    public class Stat
    {
        [SerializeField] public float CurrentValue;
        [SerializeField] public float LowerThreshold;
        [SerializeField] public float UpperThreshold;
        [SerializeField] public float PerfecValue;

        public bool IsAboveUpperThreshold() => CurrentValue > UpperThreshold;
        public bool IsBelowLowerThreshold() => CurrentValue < LowerThreshold;

        public bool IsInThreshold() => CurrentValue >= LowerThreshold && CurrentValue <= UpperThreshold;

        public void SetStat()
        {
            CurrentValue = 0;
            LowerThreshold = UnityEngine.Random.Range(0, 80);
            UpperThreshold = UnityEngine.Random.Range((int)LowerThreshold + 10, 100);
            PerfecValue = (LowerThreshold + UpperThreshold) / 2;
        }
    }

    protected Stat _toxicity = new();
    protected Stat _alcohol = new();
    protected Stat _bitterness = new();
    protected Stat _sweetness = new();
    protected Stat _sourness = new();

    public List<Stat> AllStats;

    public event Action OnClientReady;
    public event Action<bool, GradeType> OnClientSatisfied;
    public event Action OnClientFeelSick;
    public event Action OnClientDied;

    protected ClientMovement _movement;
    protected ClientVisual _visual;

    private void Awake()
    {
        AllStats = new() { _toxicity, _alcohol, _bitterness, _sweetness, _sourness };

        _movement = GetComponent<ClientMovement>();
        _visual = GetComponent<ClientVisual>();

        _movement.OnClientReady += () => OnClientReady?.Invoke();
    }
    public void Spawn(SOClient client)
    {
        SetStat();

        transform.localPosition = new Vector2(15, 0);
        _visual.Setup(GetSprites(client));

        _movement.MoveIn();
    }

    #region SETUP
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
    }
    #endregion
    public void MoveOut()
    {
        _movement.MoveOut();
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
    protected virtual void UpdateStats()
    {
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
        
        if (!CheckAdditionalConditions())
        {
            if (allAboveThreshold)
            {
                Die();
            }
            else if (isFeelingSick)
                FeelSick();
            else 
            {
                CheckSatisfy();
            }
        }
    }
    protected virtual void CheckSatisfy() 
    {
        bool isSat = true;
        foreach (var stat in AllStats)
        {
            if (!stat.IsInThreshold())
            {
                isSat = false;
                break;
            }
        }
        Satisfy(isSat);
    }
    private GradeType GetGrade()
    {
        float avaragePrerfectValue = 0;
        float avarageCurrValue = 0;

        AllStats.ForEach(stat =>
        {
            if (ConditionForGrade(stat))
            {
                avarageCurrValue += stat.CurrentValue;
                avaragePrerfectValue += stat.PerfecValue;
            }
        });

        float percentageError = Mathf.Abs(avaragePrerfectValue - avarageCurrValue) / avaragePrerfectValue * 100;
        if (percentageError  < 5)
            return GradeType.S;
        else if (percentageError  < 10)
            return GradeType.A;
        else if (percentageError  < 15)
            return GradeType.B;
        else if (percentageError  < 20)
            return GradeType.C;
        else
            return GradeType.D;
    }

    protected virtual bool ConditionForGrade(Stat stat)
    {
        return true;
    }

    protected virtual bool CheckAdditionalConditions()
    {
        return false;
    }
    protected void Satisfy(bool isSat) 
    {
        OnClientSatisfied?.Invoke(isSat, GetGrade());
    }
    protected void FeelSick()
    {
        OnClientFeelSick?.Invoke();
    }
    protected void Die()
    {
        OnClientDied?.Invoke();
    }
}

