using System.Collections.Generic;
using UnityEngine;

public class StatWindow : MonoBehaviour
{
    [SerializeField] private float _maxValue;
    [Space(10)]
    [SerializeField] private Stats _lowLimit;
    [SerializeField] private Stats _highLimit;
    [SerializeField] private Stats _ideal;

    private StatBar[] _bars;

    private void Start()
    {
        _bars = GetComponentsInChildren<StatBar>();
    }
    public void SetLimits(List<Stat> stats) 
    {
        for (int i = 0; i < stats.Count; i++)
        {
            _bars[i].MaxValue = stats[i].Max;
            _bars[i].SetLimits(stats[i].LowerThreshold, stats[i].PerfecValue, stats[i].UpperThreshold);
        }
    }

    public void SetStats(Stats stats)
    {
        for (int i = 0; i < _bars.Length; i++)
        {
            _bars[i].SetValue(stats.Array[i]);
        }
    }


}
