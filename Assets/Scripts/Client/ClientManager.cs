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
        Policeman policement = Instantiate(_policemenPref, transform);
        policement.transform.localPosition = new Vector2(15, 0);
        SetupClientEvents(policement);
        _policemen.Enqueue(policement);
    }
    private void CreateClient()
    {
        Client client = Instantiate(_clientPref, transform);
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
        OnGetStar?.Invoke(1);
        _uiDialog.ShowText("+1 star :(", () => {
            SpawnNewClient();
        });
        Debug.Log("+1 star :(");
    }
    private void OnBribeFailure()
    {
        OnGetStar?.Invoke(1);

        _uiDialog.ShowText("Ya z ne loh >:( \n +1 star!", () =>
        {
            SpawnNewClient();
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
        GameManager.Instance.Silver.ChangeValue(100);
        _uiDialog.ShowText("Give me PIVO!", () => {
            GlobalEvents.Instance.OnChangeCameraPos?.Invoke(CameraPosType.Brewery);
            GlobalEvents.Instance.OnClientStatUpdated?.Invoke(_currentClient.AllStats);
        });

        GameManager.Instance.ResetAll();
    }
    private void OnCondemnHandler()
    {
        _uiDialog.ShowCodemn("POPAVSYA!", OnCondemnConfirm);
        Debug.Log("POPAVSYA!");
    }
    private void OnClientFeelSickHandler()
    {
        GameManager.Instance.SetNewGrade(GradeType.F);
        _uiDialog.ShowText("clienty ploha", SpawnNewClient);
        Debug.Log("clienty ploha");
    }
    private void OnClientDiedHandler()
    {
        if (_currentClient is Policeman)
        {
            _uiStat.SetActive(false);
            _uiStat.ShowValues(new Stats());
            OnGetStar?.Invoke(3);
            _uiDialog.ShowText("Kinez", () => { });
            Debug.Log("Kinez");
        }
        else
        {
            OnGetStar?.Invoke(1);
            GameManager.Instance.SetNewGrade(GradeType.F); 
            _uiDialog.ShowText("The end of the day | +1 star", () => {
                _uiStat.SetActive(false);
                _uiStat.ShowValues(new Stats());
                OnDayEnd?.Invoke();
            });
            Debug.Log("The end of the day | +1 star");
        }
    }
    private void OnClientSatisfiedHandler(bool isSat, GradeType grade)
    {
        GameManager.Instance.SetNewGrade(isSat?grade:GradeType.F); 
        _uiDialog.ShowText(isSat ? $"CLIENT DOVOLEN | Grade: {grade}" : $"CLIENT NE DOVOLEN >:( \n Grade: {GradeType.F}", SpawnNewClient);
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
            _uiStat.SetActive(false);
            _uiStat.ShowValues(new Stats());
            _currentClient.MoveOut();
            if (--_clientCount <= 0)
            {
                Debug.Log("The end of the day");
                OnDayEnd?.Invoke();
                return;
            }
        }
        if (!GameManager.Instance.IsPlayState) return;
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
    public void FillBars(Stats stats)
    {
        _uiStat.ShowValues(stats,2);
    } 
    public Client GetCurrentClient()
    {
    return _currentClient; }
}
