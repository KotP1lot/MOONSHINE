using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialog : MonoBehaviour
{
    [SerializeField] GameObject _container;
    [SerializeField] UIBribe _uIBribe;

    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] Button _confirmBtn;
    [SerializeField] Button _bribeBtn;

    private Action _onConfirm;

    public void ShowCodemn(string text, Action action)
    {
        _container.SetActive(true);
        _onConfirm = action;
        _text.text = text;
        _confirmBtn.gameObject.SetActive(true);
        _bribeBtn.interactable = true;
        _bribeBtn.gameObject.SetActive(true);
    }

    public void ShowText(string text, Action action)
    {
        _text.text = text;
        _onConfirm = action;
        _container.SetActive(true);
        _bribeBtn.gameObject.SetActive(false);
        _confirmBtn.gameObject.SetActive(true);
    }
    public void ShowBribeUI() 
    {
         _uIBribe.SetActive(true);
        _uIBribe.Setup(UnityEngine.Random.Range(10,1001));
        _bribeBtn.interactable = false;
    }
    public void OnConfirmClick()
    {
        _container.SetActive(false);
        _confirmBtn.gameObject.SetActive(false);
        _bribeBtn.gameObject.SetActive(false);
        _onConfirm();
    }
}
