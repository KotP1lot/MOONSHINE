using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBribe : MonoBehaviour
{
    [SerializeField] GameObject _conteiner;

    [SerializeField] List<Sprite> _emotions;
    [SerializeField] Image _currEmotion;
    
    [SerializeField] TMP_InputField _bribeTxt;
    [SerializeField] Slider _slider;

    public event Action<bool> OnBribeResult;
    public event Action OnCancle;
    
    private int _bribeAmount;
    private int _bribeNeeded;
    private float _percent;

    public void Setup(int bribe) 
    {
        _slider.maxValue = PlayerWallet.Instance.Gold.Amount;
        ChangeValue(0);
        _bribeNeeded = bribe;
        SetActive(true);
    }
    public void ChangeValue(string value) 
    {
        int.TryParse(value, out _bribeAmount);
        _slider.value = _bribeAmount;
        ChangeEmotion();
    }
    public void ChangeValue(float value) 
    {
        _bribeAmount = (int)value;
        _bribeTxt.text = _bribeAmount.ToString();
        ChangeEmotion();
    }
    private void ChangeEmotion() 
    {
        _percent = (float)_bribeAmount/ (float)_bribeNeeded;
        if (_percent < 0.5f) _currEmotion.sprite = _emotions[0];
        else if (_percent < 1.5f) _currEmotion.sprite = _emotions[1];
        else _currEmotion.sprite = _emotions[2];
    }

    public void SetActive(bool isActive)
    {
        _conteiner.SetActive(isActive);
    }
    public void Confirm() 
    {
        int random = UnityEngine.Random.Range(1, 101);
        OnBribeResult?.Invoke(_percent * 100 >= random);
        SetActive(false);
    }
}
