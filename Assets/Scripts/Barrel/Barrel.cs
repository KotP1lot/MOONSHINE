using UnityEngine;

public class Barrel : MonoBehaviour
{
    private Stats _beerStat;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DragNDrop component) && !component.IsInHand)
        {
            if (collision.TryGetComponent(out Ingredient ingredient))
            {
                AddStat(ingredient.GetStats());
            }
        }
    }

    private void AddStat(Stats stat) 
    {
        _beerStat.Alcohol += stat.Alcohol;
        _beerStat.Sourness += stat.Sourness;
        _beerStat.Bitterness += stat.Bitterness;
        _beerStat.Sweetness += stat.Sweetness;
        _beerStat.Toxicity += stat.Toxicity;
    }
}
