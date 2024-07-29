using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPropUpgrade : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] TextMeshProUGUI _descTxt;
    [SerializeField] Image _image;
    [SerializeField] Button _buyBtn;
    [SerializeField] TextMeshProUGUI _costTxt;
    [SerializeField] List<Image> _lvlImages;
    
    [SerializeField] Sprite _reachedLvlSprite;

    public SOUpgrade SO;
    public Upgrade Upgrade;
    public event Action<SOUpgrade> OnPropClick;
    public event Action<Upgrade> OnUpgradeConfirm;
    public void Awake()
    {
        GameManager.Instance.Gold.OnResourceChanged += (i) => CheckGold();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPropClick?.Invoke(SO);
    }

    public void Setup(Upgrade upgrade) 
    { 
        Upgrade = upgrade;
        SO = upgrade.SO;
        _descTxt.text = SO.Description;
        _image.sprite = SO.Image;
        Upgrade.OnUpgrade += OnUpgrade;
        for (int i = 0; i < 3; i++) 
        {
            _lvlImages[i].enabled = i <= SO.LvlInfo.Count-1;
        }
    }
    public void LVLUp()
    {
        AudioManager.instance.Play("Roll");
        OnUpgradeConfirm?.Invoke(Upgrade);
        GameManager.Instance.Gold.Spend(SO.LvlInfo[Upgrade.CurrLvl].cost);
    }
    public void OnUpgrade()
    {
        if (Upgrade.IsMaxLevel)
        {
            _buyBtn.interactable = false;
            _costTxt.text = "MAX";
        }
        else
        {
            CheckGold();
        }
        for (int i = 0; i <= Upgrade.CurrLvl; i++) { _lvlImages[i].sprite = _reachedLvlSprite; }
    }
    private void CheckGold()
    {
        if (Upgrade.IsMaxLevel) return;
            int cost = SO.LvlInfo[Upgrade.CurrLvl + 1].cost;
        _buyBtn.interactable = GameManager.Instance.Gold.Amount >= cost;
        _costTxt.text = cost.ToString();
    }
}
