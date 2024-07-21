using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    private enum ClientType 
    {
        Client,
        Police,
        UndercoverPolice
    }
    [Header("Client")]
    [SerializeField] private int _clientCount;
    [SerializeField] private Client _clientPref;
    [SerializeField] private ClientCollection _clientCollection;
    private Client _currentClient;
    private Queue<Client> _clients = new();
    
    private Queue<ClientType> _queue = new();

    [Header("Policeman")]
    [SerializeField] private int _policemenCount;
    [SerializeField] private int _undercoverPolicemenCount;
    [SerializeField] private Policeman _policemenPref;
    [SerializeField] private ClientCollection _policemenCollection;
    private Queue<Policeman> _policemen = new();

    [Header("UI")]
    [SerializeField] UIStat _uiStat;
    

    private void Start()
    {
        CreatePolicement();
        CreatePolicement();
        CreateAndEnqueueClient();
        CreateAndEnqueueClient();
        CreateQueue();
        SpawnNewClient();
        
    }
    private void CreateQueue() 
    {
        List<ClientType> clients = new List<ClientType>(_clientCount);

        clients.AddRange(Enumerable.Repeat(ClientType.Police, _policemenCount));
        clients.AddRange(Enumerable.Repeat(ClientType.UndercoverPolice, _undercoverPolicemenCount));
        clients.AddRange(Enumerable.Repeat(ClientType.Client, _clientCount - _policemenCount - _undercoverPolicemenCount));

        _queue = new(clients.ShuffleList());
    }
    private void CreatePolicement() 
    {
        Policeman policement = Instantiate(_policemenPref, new Vector2(15, 0), Quaternion.identity);
        SetupClientEvents(policement);
        _policemen.Enqueue(policement);
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

        if (client is Policeman police)
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
        if (client is Policeman)
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
        if (client is Policeman police)
        {
            _policemenCount--;
            _policemen.Enqueue(police);
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
        switch (_queue.Dequeue()) 
        {
            case ClientType.Client:
                _currentClient = _clients.Dequeue();
                _currentClient.Spawn(_clientCollection.GetRandomClient());
                break;
            case ClientType.Police:
                _currentClient = _policemen.Dequeue();
                _currentClient.Spawn(_policemenCollection.GetRandomClient());
                break;
            case ClientType.UndercoverPolice:
                _currentClient = _policemen.Dequeue();
                _currentClient.Spawn(_clientCollection.GetRandomClient());
                break;
        }
    }
    public void Confirm() 
    {
        _currentClient.Drink(new Stats());
    }
}
