using TMPro;
using UnityEngine;

public class OutlinedText : MonoBehaviour
{
    private TextMeshProUGUI[] _texts;

    private void Start()
    {
        _texts= GetComponentsInChildren<TextMeshProUGUI>();
    }

    public void SetText(string text)
    {
        foreach (var t in _texts)
            t.text = text.RemoveBetween("<", ">");
        _texts[4].text = text;
    }
}
