using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : Aparat
{
    [SerializeField] private ClientManager _clientManager;
    [SerializeField] private StatWindow _statWindow;
    [SerializeField] private Transform _ingredientParent;
    [SerializeField] private Transform _throwOutPoint;
    [SerializeField] private Beer _pivoPrefab;
    [Space(10)]
    [SerializeField] private SpriteRenderer _barrelClosed;
    [SerializeField] private BarrelAnimation _barrelAnimation;

    public Stats _beerStat;

    private WaterShapeController _water;
    private Collider _collider;

    private void Start ()
    {
        _water = GetComponentInChildren<WaterShapeController>();
        _collider = GetComponent<Collider>();
        GlobalEvents.Instance.OnClientStatUpdated += SetupStatBar;
    }

    private void SetupStatBar(List<Stat> obj)
    {
        _statWindow.SetLimits(obj);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.parent.TryGetComponent(out Ingredient ingredient))
        {
            if (ingredient.IsUsed) return;
            AddStat(ingredient.GetStats());
            _water.AddFloater(collision.GetComponent<Rigidbody>());
            ingredient.IsUsed = true;
            Destroy(collision.GetComponent<DragNDrop3D>());
            Destroy(collision.GetComponent<CursorHover>());
            ingredient.transform.SetParent(_ingredientParent);
        }

        if (collision.transform.parent.TryGetComponent(out EssenceComponent essence))
        {
            essence.EnablePhysics(false);
            essence.transform.DOMove(_throwOutPoint.position, 1).SetEase(Ease.OutCirc).SetDelay(0.5f)
                .onComplete = ()=> { essence.EnablePhysics(true); };
        }
    }
    private void AddStat(Stats stat)
    {
        _beerStat.Alcohol += stat.Alcohol;
        _beerStat.Sourness += stat.Sourness;
        _beerStat.Bitterness += stat.Bitterness;
        _beerStat.Sweetness += stat.Sweetness;
        _beerStat.Toxicity += stat.Toxicity;

        _statWindow.SetStats(_beerStat);
    }

    public void Cook() 
    {
        GameManager.Instance.SetProcessing(true);

        _barrelAnimation.PlayAnimation(this,CreateBeer, onComplete: () =>
        {
            gameObject.SetActive(true);

            GlobalEvents.Instance.OnChangeCameraPos?.Invoke(CameraPosType.Client);

            Utility.Delay(0.2f, () =>
            {
                GlobalEvents.Instance.BeforeBeerCook?.Invoke();
                Utility.Delay(Time.deltaTime,()=>StartCoroutine(DestroyIngredients()));

                GameManager.Instance.SetProcessing(false);

                _beerStat = new();
                _statWindow.SetStats(_beerStat);
            });
        });
    }

    public Beer CreateBeer(float slideDelay)
    {
        var beer = Instantiate(_pivoPrefab);
        beer.SetStats(_beerStat,_clientManager);

        Utility.Delay(slideDelay, () => beer.Slide());

        return beer;
    }

    IEnumerator DestroyIngredients() 
    {
        Item[] childTransforms = FindObjectsOfType<Item>();
        foreach (var children in childTransforms)
        {
            Destroy(children.gameObject);
        };
        yield return null;
        GlobalEvents.Instance.OnBeerCooked?.Invoke();
    }

    public override void ChangeState(TweenCallback onComplete)
    {
        float alpha = _barrelClosed.color.a == 1 ? 0 : 1;
        _barrelClosed.DOFade(alpha, 0.3f).SetEase(Ease.OutCirc).onComplete = () =>
        {
            onComplete?.Invoke();
        };


        bool active = _ingredientParent.gameObject.activeSelf;

        foreach (var child in _ingredientParent.GetComponentsInChildren<Ingredient>())
            child.EnablePhysics(!active);

        float ingredientDelay = active ? 0.3f : 0;
        Utility.Delay(ingredientDelay, () =>
        {
            _ingredientParent.gameObject.SetActive(!active);
        });
    }
}
