using System;
using System.Collections.Generic;
using UnityEngine;

public enum GradeType
{
    F,
    D,
    C,
    B,
    A,
    S
}
[Serializable]
public class Stat
{
    [SerializeField] public int Max;
    [SerializeField] public float CurrentValue;
    [SerializeField] public float LowerThreshold;
    [SerializeField] public float UpperThreshold;
    [SerializeField] public float PerfectValue;

    public bool IsAboveUpperThreshold() => CurrentValue > UpperThreshold;
    public bool IsBelowLowerThreshold() => CurrentValue < LowerThreshold;

    public bool IsInThreshold() => CurrentValue >= LowerThreshold && CurrentValue <= UpperThreshold;

    public void SetStat(int max, float maxLowerThreshold, float stepThreshold)
    {
        CurrentValue = 0;
        Max = max;
        float lowerMax = max * maxLowerThreshold;
        float upperMin = max * stepThreshold; 
        LowerThreshold = Mathf.RoundToInt(UnityEngine.Random.Range(0, lowerMax));
        UpperThreshold = Mathf.RoundToInt(UnityEngine.Random.Range(LowerThreshold + upperMin, max));
        PerfectValue = Mathf.RoundToInt((LowerThreshold + UpperThreshold) / 2);
    }
}
public class Client : MonoBehaviour
{
    protected Stat _toxicity = new();
    protected Stat _alcohol = new();
    protected Stat _bitterness = new();
    protected Stat _sweetness = new();
    protected Stat _sourness = new();

    public List<Stat> AllStats;
    public ClientType Type;

    public event Action OnClientReady;
    public event Action<bool, GradeType> OnClientSatisfied;
    public event Action OnClientFeelSick;
    public event Action OnClientDied;

    protected ClientMovement _movement;
    protected ClientVisual _visual;

    private void Awake()
    {
        AllStats = new() {_alcohol, _toxicity, _sweetness,_bitterness, _sourness };

        _movement = GetComponent<ClientMovement>();
        _visual = GetComponent<ClientVisual>();

        _movement.OnClientReady += () => OnClientReady?.Invoke();
    }
    public void Spawn(SOClient client, int max)
    {
        SetStat(max);
        transform.localRotation = Quaternion.identity;
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

    protected void SetStat(int max)
    {
        float maxLowerThreshold = GameManager.Instance.CurrentDay.MaxLowerThreshold;
        float stepThreshold = GameManager.Instance.CurrentDay.StepThreshold;

        AllStats.ForEach(x =>
        {
            if(x == _toxicity) x.SetStat(max/2, maxLowerThreshold, stepThreshold);
            else x.SetStat(max, maxLowerThreshold, stepThreshold);
        });
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
        
        Utility.Delay(1.3f,()=>UpdateStats());
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
        float grade = 0;
        int count = 0;
        AllStats.ForEach(stat =>
        {
            if (ConditionForGrade(stat))
            {
                grade += (int)GetGrade(stat);
                count++;
            }
        });
        return (GradeType)Mathf.RoundToInt(grade/count);
    }
    protected virtual GradeType GetGrade(Stat stat) 
    {
        float max = stat.Max;
        float curr = stat.CurrentValue;
        float perf = stat.PerfectValue;
        float percentageError;

        percentageError = Mathf.Abs(perf - curr) / max * 100;
       
        if (percentageError <= 5)
            return GradeType.S;
        else if (percentageError <= 10)
            return GradeType.A;
        else if (percentageError <= 15)
            return GradeType.B;
        else if (percentageError <= 20)
            return GradeType.C;
        else if (percentageError <= 25)
            return GradeType.D;
        else
            return GradeType.F;
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
        _movement.DieAnim();
    }
}

