using System;
using UnityEngine;

[Serializable]
public class Value
{
    public event Action<int> OnResourceChanged;
    [field: SerializeField] public int Amount { get; private set; }
    public void AddAmount(int value)
    {
        Amount += value;
        OnResourceChanged?.Invoke(Amount);
    }
    public bool Spend(int value)
    {
        if (value > Amount) return false;
        Amount -= value;
        OnResourceChanged?.Invoke(Amount);
        return true;
    }
    public void ChangeValue(int value) 
    {
        Amount = value;
        OnResourceChanged?.Invoke(Amount);
    }
    public void Reset() 
    {
        Amount = 0;
        OnResourceChanged?.Invoke(Amount);
    }
}
