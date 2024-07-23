using UnityEngine;

public class Centrifuge : MonoBehaviour
{
    [SerializeField] private IngredientsManager _ingredientsManager;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter Success");
        Debug.Log("Collider Entered: " + other.name);

        Ingredient ingredient = other.GetComponent<Ingredient>();
        if (ingredient == null)
        {
            ingredient = other.GetComponentInParent<Ingredient>();
        }

        if (ingredient != null)
        {
            Debug.Log("Ingredient component found.");
            ProcessIngredient(ingredient);
        }
        else
        {
            Debug.LogError("No Ingredient component found on the entering collider.");
        }
    }

    public void ProcessIngredient(Ingredient ingredient)
    {
        Essence essence = _ingredientsManager.DestroyIngredient(ingredient);
        Debug.Log($"Created {essence.Type} essence with {essence.GetStrengthLevel()} strength.");
    }
}
