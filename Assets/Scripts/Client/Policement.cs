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
    protected override List<Sprite> GetSprites(SOClient client)
    {
        List<Sprite> sprites = CollectSprites(client, client.Accessories);

        if (client.PoliceAccessories.Count > 0)
        {
            List<Accessory> policeAccessories = client.PoliceAccessories;
            sprites.Add(policeAccessories[UnityEngine.Random.Range(0, policeAccessories.Count)].Sprite);
        }

        return sprites;
    }
}
