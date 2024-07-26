using System;
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
    [SerializeField] Transform _clientContainer;
    [SerializeField] int _maxStat;

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
    [SerializeField] UIBribe _uiBribe;
    [SerializeField] UIDialog _uiDialog;

    [Header("Temp")]
    [SerializeField] Stats _stats;

    public Action OnDayEnd;
    public Action<int> OnGetStar;
    private void Awake()
    {
        CreatePolicement();
        CreatePolicement();
        CreateClient();
        CreateClient();

        _uiBribe.OnBribeResult += OnBribeResult;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Confirm(_stats);
        }
    }
    private void OnDisable()
    {
        _uiBribe.OnBribeResult -= OnBribeResult;
        _clients.Clear();
        _policemen.Clear();

        Client[] clients = FindObjectsOfType<Client>();
        foreach (var client in clients)
        {
            RemoveClientEvents(client);
            Destroy(client.gameObject);
        }
    }
    public void StartNewDay(int clientCount, int policemenCount, int undercoverPolicemenCount) 
    {
        _clientCount = clientCount;
        _policemenCount = policemenCount;
        _undercoverPolicemenCount = undercoverPolicemenCount;

        CreateQueue();
        SpawnNewClient();
    }
    #region CREATE CLIENTS
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
        Policeman policement = Instantiate(_policemenPref, _clientContainer);
        policement.transform.localPosition = new Vector2(15, 0);
        SetupClientEvents(policement);
        _policemen.Enqueue(policement);
    }
    private void CreateClient()
    {
        Client client = Instantiate(_clientPref, _clientContainer);
        client.transform.localPosition = new Vector2(15, 0);
        SetupClientEvents(client);
        _clients.Enqueue(client);
    }
    private void SetupClientEvents(Client client)
    {
        client.OnClientReady += OnClientReadyHandler;
        client.OnClientFeelSick += OnClientFeelSickHandler;
        client.OnClientDied += OnClientDiedHandler;
        client.OnClientSatisfied += OnClientSatisfiedHandler;

        if (client is Policeman police)
        {
            police.OnCondemn += OnCondemnHandler;
        }
    }
    private void RemoveClientEvents(Client client)
    {
        client.OnClientReady -= OnClientReadyHandler;
        client.OnClientFeelSick -= OnClientFeelSickHandler;
        client.OnClientDied -= OnClientDiedHandler;
        client.OnClientSatisfied -= OnClientSatisfiedHandler;

        if (client is Policeman police)
        {
            police.OnCondemn -= OnCondemnHandler;
        }
    }
    #endregion

    #region CLIENTS HANDLERS
    private void OnBribeResult(bool obj)
    {
        if (obj)
            OnBribeSuccess();
        else
            OnBribeFailure();
    }
    private void OnCondemnConfirm() 
    {
        _uiDialog.ShowText("+1 star :(", () => {
            SpawnNewClient();
            OnGetStar?.Invoke(1);
        });
        Debug.Log("+1 star :(");
    }
    private void OnBribeFailure()
    {
        _uiDialog.ShowText("Ya z ne loh >:( \n +1 star!", () =>
        {
            SpawnNewClient();
            OnGetStar?.Invoke(1);
        });
        Debug.Log("Ya z ne loh >:( \n +1 star!");
    }
    private void OnBribeSuccess() 
    {
        _uiDialog.ShowText("ok :)", SpawnNewClient);
        Debug.Log("Sho tut?");
    }
    private void OnClientReadyHandler()
    {
        Debug.Log("SHOW STATS");
        _uiStat.ShowStats(_currentClient.AllStats);
        _uiDialog.ShowText("Give me PIVO!", () => {
            GlobalEvents.Instance.OnChangeCameraPos?.Invoke(CameraPosType.Brewery);
            GlobalEvents.Instance.OnClientStatUpdated?.Invoke(_currentClient.AllStats);
        });
    }
    private void OnCondemnHandler()
    {
        _uiDialog.ShowCodemn("POPAVSYA!", OnCondemnConfirm);
        Debug.Log("POPAVSYA!");
    }
    private void OnClientFeelSickHandler()
    {
        _uiDialog.ShowText("clienty ploha", SpawnNewClient);
        Debug.Log("clienty ploha");
    }
    private void OnClientDiedHandler()
    {
        if (_currentClient is Policeman)
        {
            _uiDialog.ShowText("Kinez", () => { });
            Debug.Log("Kinez");
        }
        else
        {
            _uiDialog.ShowText("The end of the day | +1 star", () => {
                OnGetStar?.Invoke(1);
                OnDayEnd?.Invoke();
            });
            Debug.Log("The end of the day | +1 star");
        }
    }

    private void OnClientSatisfiedHandler(bool isSat, GradeType grade)
    {
        GameManager.Instance.Gold.AddAmount(isSat
            ? grade switch
            {
                GradeType.S => 100,
                GradeType.A => 80,
                GradeType.B => 60,
                GradeType.C => 40,
                GradeType.D => 20,
                _ => 5
            }
        : 0);
        _uiDialog.ShowText(isSat ? $"CLIENT DOVOLEN | Grade: {grade}" : "CLIENT NE DOVOLEN >:(", SpawnNewClient);
    }

    #endregion
    private void SpawnNewClient()
    {
        if (_currentClient != null)
        {
            if (_currentClient is Policeman police)
            {
                _policemen.Enqueue(police);
            }
            else
            {
                _clients.Enqueue(_currentClient);
            }
            if (--_clientCount <= 0)
            {
                Debug.Log("The end of the day");
                OnDayEnd?.Invoke();
                return;
            }
            _uiStat.SetActive(false);
            _currentClient.MoveOut();
        }
        switch (_queue.Dequeue()) 
        {
            case ClientType.Client:
                _currentClient = _clients.Dequeue();
                _currentClient.Spawn(_clientCollection.GetRandomClient(), _maxStat);
                break;
            case ClientType.Police:
                _currentClient = _policemen.Dequeue();
                _currentClient.Spawn(_policemenCollection.GetRandomClient(), _maxStat);
                break;
            case ClientType.UndercoverPolice:
                _currentClient = _policemen.Dequeue();
                _currentClient.Spawn(_clientCollection.GetRandomClient(), _maxStat);
                break;
        }
    }
    public void Confirm(Stats stats) 
    {
        _currentClient.Drink(stats);
    }
}
