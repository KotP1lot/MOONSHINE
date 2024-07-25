using System.Collections;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    [SerializeField] private ClientManager _clientManager;
    [SerializeField] private StatWindow _statWindow;
    [SerializeField] private Transform _ingredientParent;

    public Stats _beerStat;

    private WaterShapeController _water;

    private void Start()
    {
        _water = GetComponentInChildren<WaterShapeController>();
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
}
