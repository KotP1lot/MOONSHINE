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
    [SerializeField] DialogSO _dialogs;

    [Header("Temp")]
    [SerializeField] private Letter _letter;
    [SerializeField] private ParticleSystem _pukeParticles;

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
        _uiDialog.ShowText(_dialogs.Condemn, () => {
            OnGetStar?.Invoke(1);
            SpawnNewClient();
        });
    }
    private void OnBribeFailure()
    {
        _uiDialog.ShowText(_dialogs.BribeNotHappy, () =>
        {
            OnGetStar?.Invoke(1);
            SpawnNewClient();
        });
    }
    private void OnBribeSuccess()
    {
        _uiDialog.ShowText(_dialogs.BribeHappy, SpawnNewClient);
    }
    private void OnClientReadyHandler()
    {
        _uiStat.ShowStats(_currentClient.AllStats);
        GameManager.Instance.Silver.ChangeValue(100);
        _uiDialog.ShowText(GetRandomDialog(_dialogs.ClientHi), () => {
            GlobalEvents.Instance.OnChangeCameraPos?.Invoke(CameraPosType.Brewery);
            GlobalEvents.Instance.OnClientStatUpdated?.Invoke(_currentClient.AllStats);
        });

        GameManager.Instance.ResetAll();
    }
    private void OnCondemnHandler()
    {
        _uiDialog.ShowCodemn(_dialogs.Popavsya, OnCondemnConfirm);
    }
    private void OnClientFeelSickHandler()
    {
        GameManager.Instance.SetNewGrade(GradeType.F);

        Utility.Delay(_pukeParticles.main.duration,()=>_uiDialog.ShowText(GetRandomDialog(_dialogs.ClientPuke), SpawnNewClient));

        _pukeParticles.transform.position = _currentClient.transform.position;
        _pukeParticles.Play();
        AudioManager.instance.Play("Puke");
    }
    private void OnClientDiedHandler()
    {
        if (_currentClient is Policeman)
        {
            _uiStat.SetActive(false);
            _uiStat.ShowValues(new Stats());
            _letter.HideLetter();
            OnGetStar?.Invoke(3);
            _uiDialog.ShowText(_dialogs.CopDied, () => { });
        }
        else
        {
            OnGetStar?.Invoke(1);
            GameManager.Instance.SetNewGrade(GradeType.F); 
            _uiDialog.ShowText(_dialogs.ClientDied, () => {
                _uiStat.SetActive(false);
                _uiStat.ShowValues(new Stats());
                _letter.HideLetter();
                OnDayEnd?.Invoke();
            });
        }
    }
    private void OnClientSatisfiedHandler(bool isSat, GradeType grade)
    {
        GameManager.Instance.SetNewGrade(isSat?grade:GradeType.F); 
        _uiDialog.ShowText(isSat ? GetRandomDialog(_dialogs.ClientHappy) : GetRandomDialog(_dialogs.ClientNotHappy), SpawnNewClient);
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
            _letter.HideLetter();
            _currentClient.MoveOut();
            if (--_clientCount <= 0)
            {
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

    private string GetRandomDialog(string[] texts)
    {
        return texts[UnityEngine.Random.Range(0, texts.Length)];
    }
}
