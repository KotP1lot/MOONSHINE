using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public Value Silver {get; private set;}
    public Value Gold {get; private set;}

    private void Awake()
    {
        Silver = new();
        Gold = new();
        Gold.AddAmount(1000);
    }
}
