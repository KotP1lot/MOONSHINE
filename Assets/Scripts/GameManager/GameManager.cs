using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public struct StatColors
{
    public Color Alcohol;
    public Color Toxicity;
    public Color Sweetness;
    public Color Bitterness;
    public Color Sourness;
    public Color NegativeValue;

    public Color[] Array
    {
        get
        {
            return new[] { Alcohol, Toxicity, Sweetness, Bitterness, Sourness };
        }
    }
}

[Serializable]
public struct RarityPrice
{
    public Rarity Rarity;
    public int Price;
}

[Serializable]
public struct GradeGold 
{
    public GradeType Grade;
    public int MinGold;
    public int MaxGold;
}
[Serializable]
public struct Finish 
{
    public FinishType FinishType;
    public Sprite Sprite;
}
public enum FinishType 
{
    Police,
    Bad,
    Normal,
    Super
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] SOUpgrade _silverUpgrade;
    [SerializeField] ClientManager _clientManager;
    [SerializeField] int _maxStars;
    [SerializeField] AparatChanger _aparatChanger;
    [SerializeField] Letter _letter;

    [Header("Stats")]
    public int HighestStat;
    public float HighestEssencePercent;
    public StatColors Colors;

    [Header("Prices")]
    [SerializeField] private int _essenceTopCost;
    [SerializeField] private RarityPrice[] _rarityPrices;
    [SerializeField] private ParticleSystem _goldParticles;

    [Header("Days")]
    [SerializeField] List<Day> _days;
    private Queue<Day> _daysQueue;
    public Day CurrentDay { get; private set; }

    [Header("Macro")]
    [SerializeField] private int _silverPerClient;
    [SerializeField] private int _startGoldAmount;
    [SerializeField] private List<GradeGold> _gradeGold;
    [SerializeField] private Letter _reputationLetter;
    private GradeType _reputation;
    private int _gradeCount;
    private int _grade;
    private int _silverBonus;

    [Header("Light")]
    [SerializeField] private List<SpriteRenderer> _lightSprite1;
    [SerializeField] private GameObject _menu;

    [Header("MainMenu")]
    [SerializeField] private Transform _mainMenu;
    [SerializeField] private RectTransform _playerStats;

    [Header("NewsPaper")]
    [SerializeField] private SpriteRenderer _newSpaper;
    [SerializeField] private CanvasGroup _newspaperButton;
    [SerializeField] private TextMeshProUGUI _newspaperButtonText;
    [SerializeField] private OutlinedText _daysText;

    [Header("Finish")]
    [SerializeField] private SpriteRenderer _fade;
    [SerializeField] private SpriteRenderer _vignette;
    [SerializeField] private CanvasGroup _restartButton;
    [SerializeField] private List<Finish> _finishes;
    public Value Silver { get; private set; }
    public Value Gold { get; private set; }
    public Value Stars { get; private set; }
    public Value Days { get; private set; }
    public bool IsPlayState { get; private set; }

    private bool _isReadyToRestart;

    private Resetter[] _resetters;

    public int GetBribe()
    {
        return UnityEngine.Random.Range(CurrentDay.MinBribe, CurrentDay.MaxBribe + 1);
    }
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
        _clientManager.OnDayEnd += OnDayEnded;
        _clientManager.OnGetStar += GetStars;
        Silver = new();
        Gold = new();
        Stars = new();
        Days = new();
        _isReadyToRestart = true;

        _resetters = FindObjectsOfType<MonoBehaviour>().OfType<Resetter>().ToArray();

        _silverUpgrade.OnUpgrade += OnUpgrade;
    }
    private void OnDisable()
    {
        _clientManager.OnDayEnd -= OnDayEnded;
        _clientManager.OnGetStar -= GetStars;
    }
    private void Update()
    {
        if (Input.anyKeyDown && _isReadyToRestart) 
        {
            MainMenu(false);
        }
    }
    public void MainMenu(bool isActive)
    {
        _isReadyToRestart = isActive;
        if (!isActive) 
        {
            Gold.ChangeValue(_startGoldAmount);
            Stars.Reset();
            Days.Reset();
            _gradeCount = 0;
            _grade = 0;
            _daysQueue = new Queue<Day>(_days);
        }
        
        if(isActive)
            _mainMenu.GetComponent<MainMenuScreen>().Start();
        else
            _mainMenu.GetComponent<MainMenuScreen>().Stop();
        
        _mainMenu.DOLocalMoveY(isActive ? 0 : 6, 1f).SetEase(Ease.InOutBack);
        _playerStats.DOAnchorPosX(isActive ? -40 : 0, 1f).SetEase(Ease.InOutBack);

        StartNewDay();
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
        //_newSpaper.transform.DOScale(0, 0.7f).SetEase(Ease.InCirc);
        //_fade.DOFade(0, 0.5f).SetEase(Ease.OutCirc);
        //_vignette.DOFade(0, 0.5f).SetEase(Ease.OutCirc).OnComplete(() => MainMenu(true));

        //_restartButton.interactable = true;
        //_restartButton.blocksRaycasts = true;
        //_restartButton.DOFade(0, 0.3f).SetEase(Ease.OutCirc);
    }
    public void End()
    {
        FinishType type;
        if (Stars.Amount >= 3) type = FinishType.Police; 
        else if ((int)_reputation < 2) type = FinishType.Bad;  // < C
        else if ((int)_reputation < 4) type = FinishType.Normal; // < A
        else type = FinishType.Super; // >= A 

        Debug.Log(type);


        LightTurn(false);
        Utility.Delay(1.5f, () =>
        {
            _fade.DOFade(0.7f, 1).SetEase(Ease.InCirc);
            _vignette.DOFade(1, 1f).SetEase(Ease.InCirc).OnComplete(() =>
            {
                ShowNews(_finishes.Find(x => x.FinishType == type).Sprite);

                Utility.Delay(1.5f, () =>
                {
                    _restartButton.DOFade(1, 0.3f).SetEase(Ease.OutCirc).onComplete =
                    () =>
                    {
                        _restartButton.interactable = true;
                        _restartButton.blocksRaycasts = true;
                    };
                });
            });
        });
    }
    public void ShowMenu(bool isActive) 
    {
        ShowNews(null);
        _newspaperButton.interactable = false;
        _newspaperButton.blocksRaycasts = false;
        _newspaperButton.DOFade(0, 0.3f).SetEase(Ease.OutCirc);

        _menu.transform.DOLocalMoveY(isActive ? 30 : 140, 1f).SetEase(Ease.InOutBack);
    }
    private void OnDayEnded() 
    {
        IsPlayState = false;
        if (_daysQueue.Count == 0) 
        {
            End();
            return; 
        }

        LightTurn(false);

        if (CurrentDay.Newspaper == null) ShowMenu(true);
        else
        {
            Utility.Delay(2f, () =>
            {
                ShowNews(CurrentDay.Newspaper);
                Utility.Delay(1.5f, () =>
                {
                    _newspaperButton.DOFade(1, 0.3f).SetEase(Ease.OutCirc).onComplete =
                    () =>
                    {
                        _newspaperButton.interactable = true;
                        _newspaperButton.blocksRaycasts = true;
                    };

                    _newspaperButtonText.text = CurrentDay.NewspaperButtonText;
                });
            });
            
        }
    }
    private void LightTurn(bool isOn) 
    {
        AudioManager.instance.Play("Lampa");
        Utility.Delay(0.5f, () =>
        {
            _lightSprite1.ForEach(x =>
            {
                x.DOFade(isOn ? 1 : 0, 1.5f).SetEase(Ease.OutFlash, 15, 1);
            }
        );
        });
    }

    private void ShowNews(Sprite newspaper)
    {
        if(newspaper==null)
        {
            _newSpaper.transform.DOScale(0, 0.3f).SetEase(Ease.OutCirc);
            return;
        }

        _newSpaper.sprite = newspaper;
        _newSpaper.transform.DOScale(1, 0.7f).SetEase(Ease.InCirc).onComplete = () =>
        {
            _newSpaper.transform.DOScale(1.5f, 0.1f).SetEase(Ease.Linear).onComplete = () =>
            _newSpaper.transform.DOScale(1f, 0.2f).SetEase(Ease.OutCirc);
        };

        _newSpaper.transform.DOLocalRotate(new Vector3(0, 0, 360*4), 0.9f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCirc);
    }

    //TEMP --------------------------------------------------------------------------------------
    public void StartNewDay()
    {
        if (_daysQueue.Count == 0)
            return;
        ShowMenu(false);
        LightTurn(true);
        CurrentDay = _daysQueue.Dequeue();
        IsPlayState = true;
        Days.AddAmount(1);
        Silver.ChangeValue(_silverPerClient);
        _clientManager.StartNewDay(CurrentDay.ClientCount,
            CurrentDay.PoliceCount,
            CurrentDay.UnderoverPoliceCount);
        _daysText.SetText(_daysQueue.Count + " DAYS LEFT");
    }
    public void GetStars(int value)
    {
        Stars.AddAmount(value);
        if (Stars.Amount >= _maxStars) 
        {
            GlobalEvents.Instance.OnGameEnded?.Invoke();
            End();
            IsPlayState = false;
            return;
        }
    }

    public void SetProcessing(bool proc)
    {
        _aparatChanger.EnableButtons(!proc);
    }

    public void ResetAll()
    {
        foreach (var reset in _resetters)
            reset.ResetValues();
    }

    public int GetPriceByRarity(Rarity rarity)
    {
        return _rarityPrices.First(x=>x.Rarity == rarity).Price;
    }

    public int GetEssenceCost(float strength)
    {
        return Mathf.RoundToInt(strength/HighestEssencePercent * _essenceTopCost);
    }

    public void SetNewGrade(GradeType grade)
    {
        _letter.ShowLetter(grade);
        GradeGold gold = _gradeGold.Find(x => x.Grade == grade);
        ObtainGold(UnityEngine.Random.Range(gold.MinGold, gold.MaxGold));
        _grade += (int)grade;
        _gradeCount++;
        _reputation = (GradeType) Mathf.RoundToInt((float)_grade / (float)_gradeCount);
        _reputationLetter.SetLetter(_reputation);
    }

    public void ObtainGold(int amount)
    {
        if (amount == 0) return;

        float highestIncome = _gradeGold[_gradeGold.Count-1].MaxGold;
        var percent = amount / highestIncome;
        int coinCount = Mathf.CeilToInt(Mathf.Lerp(1, 50, percent));

        var burst = _goldParticles.emission.GetBurst(0);
        burst.cycleCount = coinCount;
        _goldParticles.emission.SetBurst(0, burst);
        _goldParticles.Play();

        float duration = _goldParticles.main.duration + 0.05f * coinCount;
        int goldPerCoin = Mathf.CeilToInt(amount / coinCount);
        var estimatedGoldAmount = goldPerCoin * coinCount;

        Utility.Delay(3.5f,()=> Utility.Repeat(0.05f,coinCount,()=>Gold.AddAmount(goldPerCoin)));
        Utility.Delay(duration + 0.05f, () => 
        {
            Debug.Log(amount - estimatedGoldAmount);
            if(estimatedGoldAmount < amount)
                Gold.AddAmount(amount - estimatedGoldAmount);
        });
    }

    public void SilverReset()
    {
        Silver.ChangeValue(_silverPerClient + _silverBonus);
    }

    private void OnUpgrade(Upgrade upgrade)
    {
        _silverBonus = _silverUpgrade.LvlInfo[upgrade.CurrLvl].bonus;
    }
}
