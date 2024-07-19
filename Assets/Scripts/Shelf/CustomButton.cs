using UnityEngine;
using UnityEngine.Events;

public class CustomButton : MonoBehaviour
{
    public UnityEvent OnClick;

    public void OnMouseDown()
    {
        OnClick?.Invoke();
    }
}
