using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClientVariant", menuName = "Client/ClientVariant")]
public class SOClient : ScriptableObject
{
    public Sprite BaseSprite;
    public List<Accessory> Accessories;
}
public enum AccessoryType
{
    Hat,
    Chest
}
[Serializable]
public struct Accessory 
{
    public AccessoryType Type;
    public Sprite Sprite;
}