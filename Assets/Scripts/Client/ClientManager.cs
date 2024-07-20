using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    [Header("Client")]
    [SerializeField] private int _cClient;
    [SerializeField] private Client _clientPref;
    [SerializeField] private ClientCollection _clientCollection;
    private Queue<Client> _clients = new();

    [Header("Policement")]
    [SerializeField] private int _cPolicement;
    [SerializeField] private Policement _policePref;
    private Policement policement;

    private void Start()
    {
        CreatePolicement();
        CreateAndEnqueueClient();
        CreateAndEnqueueClient();
        SpawnNewClient();
    }
    private void CreatePolicement() 
    {
        policement = Instantiate(_policePref, new Vector2(15, 0), Quaternion.identity);
        SetupClientEvents(policement);
    }
    private void CreateAndEnqueueClient()
    {
        Client client = Instantiate(_clientPref, new Vector2(15, 0), Quaternion.identity);
        SetupClientEvents(client);
        _clients.Enqueue(client);
    }
    private void SetupClientEvents(Client client)
    {
        client.OnClientSatisfied += OnClientSatisfied;
        client.OnClientDied += OnClientDied;
        client.OnClientReady += OnClientReady;

        if (client is Policement police)
        {
            police.OnCondemn += Policement_OnCondemn;
        }
    }
    private void OnClientReady(Client obj)
    {
        Debug.Log("Show stat?");
    }

    private void OnClientDied(Client client)
    {
        if (client is Policement)
            Debug.Log("Kinez");
        else
            Debug.Log("The end of the day | +1 star");
    }

    private void Policement_OnCondemn()
    {
        Debug.Log("+1 star");
    }
    private void OnClientSatisfied(Client client)
    {
        client.transform.position = new Vector2(15, 0);
        if(client is not Policement) _clients.Enqueue(client);
        SpawnNewClient();
    }

    private void SpawnNewClient()
    {

        _clients.Dequeue().Spawn(_clientCollection.GetRandomClient());
    }
}
