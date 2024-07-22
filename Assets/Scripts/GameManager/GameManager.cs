using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] int _maxStars;
    public PlayerWallet PlayerWallet;
    public int Stars { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void GetStars(int value) 
    {
        Stars += value;
        if (Stars >= _maxStars) 
        {
            Debug.Log("KINEZ");
            return;
        }
    }
}
