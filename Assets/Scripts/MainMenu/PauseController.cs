using System;
using UnityEngine;
using UnityEngine.Events;

public class PauseController : MonoBehaviour
{
    [SerializeField] private UnityEvent _onPauseOpen;
    [SerializeField] private UnityEvent _onPauseClose;
    public static Action pauseChanger;

    private bool _isActive = false;

    public bool IsActive
    {
        set
        {
            _isActive = value;

            if (_isActive)
            {
                _onPauseOpen.Invoke();
            }
            else
            {
                _onPauseClose.Invoke();
            }

            Time.timeScale = Convert.ToInt32(!_isActive);
        }
        get
        {
            return _isActive;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseChanger?.Invoke();
            IsActive = !_isActive;
        }
    }
}
