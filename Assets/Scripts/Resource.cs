using System;
using UnityEngine;

[Serializable]
public class Resource
{
    public event Action OnResourceChanged;
    [field: SerializeField] public int Amount { get; private set; }
    public void AddAmount(int value)
    {
        Amount += value;
        OnResourceChanged?.Invoke();
    }
    public bool Spend(int value)
    {
        if (value > Amount) return false;
        Amount -= value;
        OnResourceChanged?.Invoke();
        return true;
    }
}
