using System;
using System.Collections.Generic;
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
        if (_alcohol.CurrentValue > 0)
            Condemn();
    }

    public override void Spawn(List<Sprite> sprites)
    {
        _movement.MoveIn();
        SetStat();
    }
}
