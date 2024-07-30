using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SliderText : MonoBehaviour
{
    private TextMeshProUGUI _text;

    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    public void SetText(float value)
    {
        _text.text = value.ToString();
    }
}
