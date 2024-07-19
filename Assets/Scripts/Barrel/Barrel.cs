using DG.Tweening;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public Stats _beerStat;
   
    public float waveHeight = 2f; // Висота хвилі
    public float waveDuration = 1f; // Час на підйом і спуск
    public void AddIngredient(Ingredient ingredient)
    {
        ingredient.SetParent(transform);
        ingredient.transform.localPosition = new Vector2(ingredient.transform.localPosition.x, 0);
        ingredient.SetRbType(RigidbodyType2D.Static);
        ingredient.GetComponent<DragNDrop>().Deactivate();

        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(startPos.x, startPos.y + waveHeight, startPos.z);

        ingredient.transform.DOMoveY(targetPos.y, waveDuration)
        .SetEase(Ease.InOutSine) // Плавний підйом і спуск
        .SetLoops(-1, LoopType.Yoyo);

        AddStat(ingredient.GetStats());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Ingredient ingredient))
            AddIngredient(ingredient);
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
