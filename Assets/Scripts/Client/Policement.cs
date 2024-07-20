using System;
using UnityEngine;

public class Policement : Client
{
    public event Action OnCondemn;
    private void Condemn() 
    {
        Debug.Log("popa vsya");
    }
    protected override void UpdateStats()
    {
        base.UpdateStats();
        if (alcohol.currentValue > 0)
            Condemn();
    }
}
