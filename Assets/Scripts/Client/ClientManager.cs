using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    [Header("Client")]
    [SerializeField] private int _clientCount;
    [SerializeField] private Client _clientPref;
    [SerializeField] private ClientCollection _clientCollection;
    private Client _currentClient;
    private Queue<Client> _clients = new();
    private Queue<bool> _queue = new();

    [Header("Policement")]
    [SerializeField] private int _policementCount;
    [SerializeField] private Policement _policePref;
    private Policement _policement;

    [Header("UI")]
    [SerializeField] UIStat _uiStat;

    private void Start()
    {
        CreatePolicement();
        CreateAndEnqueueClient();
        CreateAndEnqueueClient();
        CreateQueue();
        SpawnNewClient();
        
    }
    private void CreateQueue() 
    {
        List<bool> clients = new List<bool>(_clientCount);
        for (int i = 0; i < _clientCount; i++) 
        {
            clients.Add(i <= _policementCount - 1);
        };
        Utility.ShuffleList(clients);
        _queue = new(clients);
    }
    private void CreatePolicement() 
    {
        _policement = Instantiate(_policePref, new Vector2(15, 0), Quaternion.identity);
        SetupClientEvents(_policement);
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
        _uiStat.ShowStats(obj.AllStats);
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
        if (client is Policement)
        {
            _policementCount--;
        }
        else
        {
            _clients.Enqueue(client);
        }
        if (--_clientCount <= 0)
        {
            Debug.Log("The end of the day");
            return;
        }
        _uiStat.SetActive(false);
        SpawnNewClient();
    }

    private void SpawnNewClient()
    {
        if (_queue.Dequeue())
        {
            SpawnPolice();
            return;
        }
        _currentClient = _clients.Dequeue();
        _currentClient.Spawn(_clientCollection.GetRandomClient());
    }
    private void SpawnPolice()
    {
        _policement.Spawn(_clientCollection.GetRandomClient());
        _currentClient = _policement;
    }
    public void Confirm() 
    {
        _currentClient.Drink(new Stats());
    }
}
