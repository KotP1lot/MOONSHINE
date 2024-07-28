using System;
using System.Collections.Generic;
using UnityEngine;

public class UIUpgrade : MonoBehaviour
{
    [SerializeField] UIUpgradeInfo _info;

    [Header("Prop")]
    [SerializeField] Transform _propContainer;
    [SerializeField] List<SOUpgrade> _upgradeInfos;
    [SerializeField] UIPropUpgrade _propPrefab;
   
    private List<UIPropUpgrade> _props = new();
    private List<Upgrade> _upgrades = new();
    private void Start()
    {
        _info.Setup(null);
        foreach (var upgrade in _upgradeInfos) 
        {
            UIPropUpgrade ui = Instantiate(_propPrefab, _propContainer);
            Upgrade up = new(-1, upgrade, false);
            _upgrades.Add(up);
            ui.Setup(up);
            ui.OnPropClick += OnClickProp;
            ui.OnUpgradeConfirm += OnPurshConfirm;
            _props.Add(ui);
            ui.gameObject.SetActive(false);
        }
        _props[0].Upgrade.IsUnlocked = true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) ShowUpgrades();
    }
    private void ShowUpgrades()
    {
        _props.Sort((x, y) => x.Upgrade.IsMaxLevel.CompareTo(y.Upgrade.IsMaxLevel));

        for (int i = 0; i < _props.Count; i++)
        {
            _props[i].transform.SetSiblingIndex(i);
            _props[i].gameObject.SetActive(_props[i].Upgrade.IsUnlocked);
        }
    }
    private void OnPurshConfirm(Upgrade upgrade) 
    {
        if (upgrade.CurrLvl == -1) 
        {
            foreach (var unlock in upgrade.SO.Unlock) 
                unlock.OnUnlock?.Invoke();
            foreach (var unlock in upgrade.SO.AlsoUpgrade) 
            {
                OnPurshConfirm(_upgrades.Find(x => x.SO == unlock));
            }
        }
        upgrade.CurrLvl++;
        upgrade.UpgradeObj();
        ShowUpgrades();
    }
    private void OnClickProp(SOUpgrade so) 
    {
        _info.Setup(so);
    }
}
public class Upgrade 
{
    public int CurrLvl;
    public SOUpgrade SO;
    public bool IsUnlocked;
    public Action OnUpgrade;
    public Upgrade(int currLvl, SOUpgrade sO, bool isUnlocked)
    {
        CurrLvl = currLvl;
        SO = sO;
        IsUnlocked = isUnlocked;
        SO.OnUnlock += Unlock;
    }
    public void UpgradeObj() 
    {
        OnUpgrade?.Invoke();
        SO.OnUpgrade?.Invoke(this);
    }
    private void Unlock() 
    {
        IsUnlocked = true;
    }
    public bool IsMaxLevel => CurrLvl >= 2;
} 
