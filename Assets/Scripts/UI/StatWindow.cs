using System.Collections;
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

        foreach (var bar in _bars)
            bar.MaxValue = _maxValue;
        
        for(int i = 0;  i < _bars.Length; i++)
        {
            _bars[i].SetLimits(_lowLimit.Array[i], _ideal.Array[i],_highLimit.Array[i]);
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
