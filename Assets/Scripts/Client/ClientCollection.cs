using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ClientCollection", menuName ="Client/ClientCollection")]
public class ClientCollection : ScriptableObject
{
    public List<SOClient> customers;
    public List<Sprite> GetRandomClient()
    {
        SOClient customer = customers[UnityEngine.Random.Range(0, customers.Count)];
        List<Sprite> sprite = new() { customer.BaseSprite };
        AccessoryType[] accessoryTypes = (AccessoryType[])Enum.GetValues(typeof(AccessoryType));
        foreach (AccessoryType type in accessoryTypes)
        {
            List<Accessory> accessory = customer.Accessories.FindAll(x => x.Type == type);
            if (accessory.Count == 0) continue;
            int random = UnityEngine.Random.Range(0, accessory.Count + 1);
            if (random == accessory.Count) continue;
            else
            {
                sprite.Add(accessory[random].Sprite);
            }
        }
        return sprite;
    }
}