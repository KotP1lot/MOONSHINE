using System;
using System.Collections.Generic;
using UnityEngine;
public enum UpgradeType 
{
    Combinator,
    Shelf,
    E_Shelf,
    Centrifuge,
}
[CreateAssetMenu()]
public class SOUpgrade : ScriptableObject
{
    public UpgradeType Type;
    public Sprite Image;
    public string Description;
    public List<LvlInfo> LvlInfo;
    public List<SOUpgrade> Unlock;
    public List<SOUpgrade> AlsoUpgrade;
    public Action OnUnlock;
    public Action<Upgrade> OnUpgrade;
}
[Serializable]
public struct LvlInfo 
{
    public int bonus;
    public string describe;
    public int cost;
}
