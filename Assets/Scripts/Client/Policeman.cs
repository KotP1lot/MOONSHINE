using System;
using System.Collections.Generic;
using UnityEngine;

public class Policeman : Client
{
    public event Action OnCondemn;

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

    protected override bool CheckAdditionalConditions()
    {
        if (_alcohol.CurrentValue > 0)
        {
            Condemn();
            return true;
        }
        return false;
    }
    private void Condemn()
    {
        OnCondemn?.Invoke();
    }
}
