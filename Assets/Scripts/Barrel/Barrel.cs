using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Barrel : Aparat
{
    [SerializeField] private ClientManager _clientManager;
    [SerializeField] private StatWindow _statWindow;
    [SerializeField] private Transform _ingredientParent;
    [SerializeField] private Transform _throwOutPoint;
    [Space(10)]
    [SerializeField] private SpriteRenderer _barrelClosed;

    public Stats _beerStat;

    private WaterShapeController _water;
    private Collider _collider;

    private void Start()
    {
        _water = GetComponentInChildren<WaterShapeController>();
        _collider = GetComponent<Collider>();
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
        GlobalEvents.Instance.OnChangeCameraPos?.Invoke(CameraPosType.Client);
        _clientManager.Confirm(_beerStat);
       
        _beerStat = new();
        _statWindow.SetStats(_beerStat);
        GlobalEvents.Instance.BeforeBeerCook?.Invoke();
        StartCoroutine(DestroyIngredients());
    }
    IEnumerator DestroyIngredients() 
    {
        Ingredient[] childTransforms = FindObjectsOfType<Ingredient>();
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
        float ingredientDelay = active ? 0.3f : 0;
        _ingredientParent.DOScaleZ(1, ingredientDelay).onComplete = () =>
        {
            _ingredientParent.gameObject.SetActive(!active);
        };
    }
}
