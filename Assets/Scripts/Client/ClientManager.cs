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

    [Header("Policement")]
    [SerializeField] private int _policementCount;
    [SerializeField] private int _undercoverPoliceCount;
    [SerializeField] private Policement _policePref;
    [SerializeField] private ClientCollection _policeCollection;
    private Queue<Policement> _policements = new();

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

        clients.AddRange(Enumerable.Repeat(ClientType.Police, _policementCount));
        clients.AddRange(Enumerable.Repeat(ClientType.UndercoverPolice, _undercoverPoliceCount));
        clients.AddRange(Enumerable.Repeat(ClientType.Client, _clientCount - _policementCount - _undercoverPoliceCount));

        _queue = new(clients.ShuffleList());
    }
    private void CreatePolicement() 
    {
        Policement policement = Instantiate(_policePref, new Vector2(15, 0), Quaternion.identity);
        SetupClientEvents(policement);
        _policements.Enqueue(policement);
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
        _currentClient = _clients.Dequeue();
        _currentClient.Spawn(_clientCollection.GetRandomClient());
    }
    private void SpawnPolice()
    {
    }
    public void Confirm() 
    {
        _currentClient.Drink(new Stats());
    }
}
