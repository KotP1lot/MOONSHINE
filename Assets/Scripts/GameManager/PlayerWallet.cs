using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public Resource Silver {get; private set;}
    public Resource Gold {get; private set;}

    private void Awake()
    {
        Silver = new();
        Gold = new();
        Gold.AddAmount(1000);
    }
}
