using UnityEngine;

public class Barrel : MonoBehaviour
{
    public Stats _beerStat;
    public void AddIngredient(Ingredient ingredient)
    {
        ingredient.SetParent(transform);
        ingredient.transform.localPosition = new Vector2(ingredient.transform.localPosition.x, 0);
        ingredient.SetKinematic(true);

        AddStat(ingredient.GetStats());
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.parent.TryGetComponent(out Ingredient ingredient))
        {
            if (ingredient.IsUsed) return;
            AddIngredient(ingredient);
            ingredient.IsUsed = true;
            Destroy(collision.GetComponent<DragNDrop3D>());
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
