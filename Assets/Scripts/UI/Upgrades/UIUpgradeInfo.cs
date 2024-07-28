using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIUpgradeInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _nameTxt;
    [SerializeField] TextMeshProUGUI _unlockTxt;
    [SerializeField] List<TextMeshProUGUI> _lvlTxt;
    public void Setup(SOUpgrade info)
    {
        if (info == null)
        {
            _nameTxt.text = "Select";
            _unlockTxt.enabled = false;
            foreach (var txt in _lvlTxt)
            {
                txt.enabled = false;
            }
        }
        else
        {
            _nameTxt.text = info.Type.ToString();
            for (int i = 0; i < _lvlTxt.Count; i++)
            {
                _lvlTxt[i].enabled = true;
                _lvlTxt[i].text = $"LVL {i + 1}: {info.LvlInfo[i].describe}";
            }
            if (info.Unlock.Count > 0)
            {
                _unlockTxt.enabled = true;
                string unlockInfo = "Unlock: ";
                List<string> unlockTypes = new List<string>();
                foreach (var unl in info.Unlock)
                {
                    unlockTypes.Add(unl.Type.ToString());
                }

                unlockInfo += string.Join(", ", unlockTypes) + ".";

                _unlockTxt.text = unlockInfo;
            }
            else 
            {
                _unlockTxt.text = "";
            }
        }
    }
}
