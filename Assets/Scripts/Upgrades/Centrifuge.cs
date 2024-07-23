using System;
using UnityEngine;

public class Centrifuge : MonoBehaviour
{
    [Header("Essence Prefabs")]
    [SerializeField] private GameObject alcoholEssencePrefab;
    [SerializeField] private GameObject toxicityEssencePrefab;
    [SerializeField] private GameObject sweetnessEssencePrefab;
    [SerializeField] private GameObject bitternessEssencePrefab;
    [SerializeField] private GameObject sournessEssencePrefab;

    private void OnTriggerEnter(Collider other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();
        if (ingredient == null)
        {
            ingredient = other.GetComponentInParent<Ingredient>();
        }

        if (ingredient != null)
        {
            ProcessIngredient(ingredient);
        }
        else
        {
            Debug.Log("No Ingredient component found on the entering collider.");
        }
    }

    private void ProcessIngredient(Ingredient ingredient)
    {
        if (ingredient != null)
        {
            Essence essence = CreateEssenceFromIngredient(ingredient);

            // Создание объекта эссенции
            GameObject essenceObject = CreateEssenceObject(essence);
            if (essenceObject != null)
            {
                essenceObject.transform.position = ingredient.transform.position;
                essenceObject.transform.rotation = Quaternion.identity;
            }

            // Уничтожение ингредиента
            Destroy(ingredient.gameObject);

            Debug.Log($"Created {essence.Type} essence with {essence.GetStrengthLevel()} strength.");
        }
        else
        {
            Debug.LogError("Ingredient is null.");
        }
    }

    private Essence CreateEssenceFromIngredient(Ingredient ingredient)
    {
        // Логика создания эссенции
        Stats stats = ingredient.GetStats();
        Array values = Enum.GetValues(typeof(Essence.EssenceType));
        Essence.EssenceType randomEssenceType = (Essence.EssenceType)values.GetValue(UnityEngine.Random.Range(0, values.Length));

        float strength = randomEssenceType switch
        {
            Essence.EssenceType.Alcohol => stats.Alcohol,
            Essence.EssenceType.Toxicity => stats.Toxicity,
            Essence.EssenceType.Sweetness => stats.Sweetness,
            Essence.EssenceType.Bitterness => stats.Bitterness,
            Essence.EssenceType.Sourness => stats.Sourness,
            _ => 0f,
        };

        Essence essence = ScriptableObject.CreateInstance<Essence>();
        essence.Type = randomEssenceType;
        essence.Strength = strength;

        return essence;
    }

    private GameObject CreateEssenceObject(Essence essence)
    {
        GameObject prefab = essence.Type switch
        {
            Essence.EssenceType.Alcohol => alcoholEssencePrefab,
            Essence.EssenceType.Toxicity => toxicityEssencePrefab,
            Essence.EssenceType.Sweetness => sweetnessEssencePrefab,
            Essence.EssenceType.Bitterness => bitternessEssencePrefab,
            Essence.EssenceType.Sourness => sournessEssencePrefab,
            _ => null
        };

        if (prefab != null)
        {
            GameObject essenceObject = Instantiate(prefab);
            EssenceComponent essenceComponent = essenceObject.GetComponent<EssenceComponent>();
            essenceComponent.SetEssence(essence);
            return essenceObject;
        }

        Debug.LogError("No prefab found for the essence type.");
        return null;
    }
}
