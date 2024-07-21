using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public Resource Silver {get; private set;}
    public Resource Gold {get; private set;}

    public static PlayerWallet Instance { get; private set;}

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

        Silver = new();
        Gold = new();
    }
}
