using UnityEngine;
using UnityEngine.UI;

public class UIStar : MonoBehaviour
{
    [SerializeField] Image _back;
    public void SetActive(bool isActive) => _back.enabled = isActive;
}
