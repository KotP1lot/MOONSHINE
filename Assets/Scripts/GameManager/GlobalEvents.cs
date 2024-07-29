using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEvents: MonoBehaviour
{
    public static GlobalEvents Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    public Action OnBeerCooked;
    public Action BeforeBeerCook;
    public Action<CameraPosType> OnChangeCameraPos;
    public Action<List<Stat>> OnClientStatUpdated;
    public Action OnGameEnded;
}
