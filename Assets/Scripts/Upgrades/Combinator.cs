//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public class Combinator : MonoBehaviour
//{
//    [Serializable]
//    private struct Combination
//    {
//        public SOIngredient Ingredient1;
//        public SOIngredient Ingredient2;
//        public SOIngredient Result;
//    }

//    [SerializeField] private List<Combination> _availableCombinations;
//    [SerializeField] private List<SOIngredient> _ingredients;
//    [SerializeField] private int _buyingPrice;
//    [SerializeField] private int _usesPerClient;
//    [SerializeField] private int _maxUsesPerClient;
//    [SerializeField] private int _upgradeCost;
//    [SerializeField] private int _upgradeCostModifier;
//    [SerializeField] private Value _money;

//    private bool _isUnlocked = false;

//    [SerializeField] private GameObject alcoholEssencePrefab;
//    [SerializeField] private GameObject toxicityEssencePrefab;
//    [SerializeField] private GameObject sweetnessEssencePrefab;
//    [SerializeField] private GameObject bitternessEssencePrefab;
//    [SerializeField] private GameObject sournessEssencePrefab;

//    private void OnTriggerEnter(Collider other)
//    {
//        Debug.Log("Enter Success");
//        Debug.Log("Collider Entered: " + other.name);

//        Ingredient ingredient = other.GetComponent<Ingredient>();
//        EssenceComponent essenceComponent = other.GetComponent<EssenceComponent>();

//        if (ingredient == null)
//        {
//            ingredient = other.GetComponentInParent<Ingredient>();
//        }
//        if (essenceComponent == null)
//        {
//            essenceComponent = other.GetComponentInParent<EssenceComponent>();
//        }

//        if (ingredient != null && essenceComponent != null)
//        {
//            Essence essence = essenceComponent.GetEssence();
//            Debug.Log($"Combining ingredient: {ingredient.Data.name} with essence: {essence.Type}");

//            if (TryCombineIngredientWithEssence(ingredient.Data, essence, out SOIngredient result))
//            {
//                Debug.Log("Combination successful.");

//                GameObject resultObject = Instantiate(GetPrefabForIngredient(result), transform.position, Quaternion.identity);
//                Destroy(ingredient.gameObject);
//                Destroy(essenceComponent.gameObject);
//            }
//            else
//            {
//                Debug.LogError("Combination failed.");
//            }
//        }
//        else if (ingredient != null && other.GetComponent<Ingredient>() != null)
//        {
//            Ingredient otherIngredient = other.GetComponent<Ingredient>();
//            Debug.Log($"Combining ingredient: {ingredient.Data.name} with other ingredient: {otherIngredient.Data.name}");

//            if (TryCombine(ingredient.Data, otherIngredient.Data, out SOIngredient result))
//            {
//                Debug.Log("Combination successful.");

//                GameObject resultObject = Instantiate(GetPrefabForIngredient(result), transform.position, Quaternion.identity);
//                Destroy(ingredient.gameObject);
//                Destroy(otherIngredient.gameObject);
//            }
//            else
//            {
//                Debug.LogError("Combination failed.");
//            }
//        }
//        else
//        {
//            Debug.LogError("No valid combination found.");
//        }
//    }

//    private GameObject GetPrefabForIngredient(SOIngredient ingredient)
//{
//    // Implement logic to return the correct prefab based on the ingredient
//    return null; // Placeholder
//}


//    public bool TryCombine(SOIngredient ingredient1Index, SOIngredient ingredient2Index, out SOIngredient result)
//    {
//        result = null;

//        if (!_isUnlocked)
//        {
//            Debug.Log("Combinator is not unlocked.");
//            return false;
//        }

//        Debug.Log($"Trying to combine: Ingredient1 = {ingredient1Index.name}, Ingredient2 = {ingredient2Index.name}");

//        foreach (var combination in _availableCombinations)
//        {
//            Debug.Log($"Checking combination: Ingredient1 = {combination.Ingredient1.name}, Ingredient2 = {combination.Ingredient2.name}");

//            if ((combination.Ingredient1 == ingredient1Index && combination.Ingredient2 == ingredient2Index) ||
//                (combination.Ingredient1 == ingredient2Index && combination.Ingredient2 == ingredient1Index))
//            {
//                Debug.Log("Combination found!");
//                result = combination.Result;
//                return true;
//            }
//        }

//        Debug.Log("No valid combination found.");
//        return false;
//    }



//    private bool TryCombineIngredientWithEssence(SOIngredient ingredient, Essence essence, out SOIngredient result)
//    {
//        result = null;

//        if (ingredient == null || essence == null)
//        {
//            return false;
//        }

//        // Calculate enhancement percentage based on essence strength
//        float enhancementPercentage = essence.GetStrengthLevel() switch
//        {
//            "Weak" => 0.1f,
//            "Moderate" => 0.2f,
//            "Strong" => 0.3f,
//            "Epic" => 0.5f,
//            _ => 0f
//        };

//        // Create a new enhanced ingredient
//        SOIngredient enhancedIngredient = ScriptableObject.CreateInstance<SOIngredient>();
//        enhancedIngredient.CopyFrom(ingredient);  // Copy base properties
//        enhancedIngredient.IsEnhanced = true;

//        // Apply enhancement
//        Stats stats = enhancedIngredient.Stats;
//        switch (essence.Type)
//        {
//            case Essence.EssenceType.Alcohol:
//                stats.Alcohol += stats.Alcohol * enhancementPercentage;
//                break;
//            case Essence.EssenceType.Toxicity:
//                stats.Toxicity += stats.Toxicity * enhancementPercentage;
//                break;
//            case Essence.EssenceType.Sweetness:
//                stats.Sweetness += stats.Sweetness * enhancementPercentage;
//                break;
//            case Essence.EssenceType.Bitterness:
//                stats.Bitterness += stats.Bitterness * enhancementPercentage;
//                break;
//            case Essence.EssenceType.Sourness:
//                stats.Sourness += stats.Sourness * enhancementPercentage;
//                break;
//        }
//        enhancedIngredient.Stats = stats;

//        result = enhancedIngredient;
//        return true;
//    }


//    public bool CanUse(int currentUses)
//    {
//        return _isUnlocked && currentUses < _usesPerClient;
//    }

//    public bool TryUpgrade()
//    {
//        if (_usesPerClient < _maxUsesPerClient)
//        {
//            _money.Spend(_upgradeCost);
//            _upgradeCost *= -_upgradeCostModifier;
//            _usesPerClient++;
//            return true;
//        }
//        return false;
//    }

//    public void Unlock()
//    {
//        _isUnlocked = true;
//    }

//}
using System;
using System.Collections.Generic;
using UnityEngine;

public class Combinator : MonoBehaviour
{
    [Serializable]
    private struct Combination
    {
        public SOIngredient Ingredient1;
        public SOIngredient Ingredient2;
        public SOIngredient Result;
    }

    [SerializeField] private List<Combination> _availableCombinations;

    private Ingredient _firstIngredient;
    private EssenceComponent _firstEssenceComponent;
    private bool _firstObjectDetected = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter Success");
        Debug.Log("Collider Entered: " + other.name);

        // Attempt to get Ingredient and EssenceComponent from the current or parent object
        Ingredient ingredient = other.GetComponent<Ingredient>() ?? other.GetComponentInParent<Ingredient>();
        EssenceComponent essenceComponent = other.GetComponent<EssenceComponent>() ?? other.GetComponentInParent<EssenceComponent>();

        if (!_firstObjectDetected)
        {
            if (ingredient != null)
            {
                _firstIngredient = ingredient;
                _firstObjectDetected = true;
                Debug.Log("First ingredient detected: " + ingredient.Data.name);
            }
            else if (essenceComponent != null)
            {
                _firstEssenceComponent = essenceComponent;
                _firstObjectDetected = true;
                Debug.Log("First essence detected: " + essenceComponent.GetEssence().Type);
            }
            else
            {
                Debug.LogError("First object is neither ingredient nor essence. Object name: " + other.name);
            }
        }
        else
        {
            // Second object detected
            if (_firstIngredient != null && essenceComponent != null)
            {
                Debug.Log("Attempting to combine ingredient and essence.");
                Essence essence = essenceComponent.GetEssence();
                if (TryCombineIngredientWithEssence(_firstIngredient.Data, essence, out SOIngredient result))
                {
                    Debug.Log("Combination successful.");
                    InstantiateResult(result);
                    Destroy(_firstIngredient.gameObject);
                    Destroy(essenceComponent.gameObject);
                }
                else
                {
                    Debug.LogError("Combination failed.");
                }
            }
            else if (_firstIngredient != null && ingredient != null)
            {
                Debug.Log("Attempting to combine two ingredients.");
                if (TryCombine(_firstIngredient.Data, ingredient.Data, out SOIngredient result))
                {
                    Debug.Log("Combination successful.");
                    InstantiateResult(result);
                    Destroy(_firstIngredient.gameObject);
                    Destroy(ingredient.gameObject);
                }
                else
                {
                    Debug.LogError("Combination failed.");
                }
            }
            else
            {
                Debug.LogError("Second object is neither valid ingredient nor essence. Object name: " + other.name);
            }

            // Reset state after processing
            _firstIngredient = null;
            _firstEssenceComponent = null;
            _firstObjectDetected = false;
        }
    }

    private void InstantiateResult(SOIngredient result)
    {
        GameObject resultObject = Instantiate(GetPrefabForIngredient(result), transform.position, Quaternion.identity);
        Debug.Log("Result ingredient instantiated: " + result.name);
        // Additional logic for placing or configuring the new ingredient
    }

    private GameObject GetPrefabForIngredient(SOIngredient ingredient)
    {
        // Return the appropriate prefab for the ingredient
        // Implement based on your game logic
        return null;
    }

    public bool TryCombine(SOIngredient ingredient1Index, SOIngredient ingredient2Index, out SOIngredient result)
    {
        result = null;
        Debug.Log("Trying to combine: " + ingredient1Index.name + " and " + ingredient2Index.name);

        foreach (var combination in _availableCombinations)
        {
            if ((combination.Ingredient1 == ingredient1Index && combination.Ingredient2 == ingredient2Index) ||
                (combination.Ingredient1 == ingredient2Index && combination.Ingredient2 == ingredient1Index))
            {
                result = combination.Result;
                Debug.Log("Found valid combination: " + result.name);
                return true;
            }
        }

        Debug.LogError("No valid combination found.");
        return false;
    }

    private bool TryCombineIngredientWithEssence(SOIngredient ingredient, Essence essence, out SOIngredient result)
    {
        result = null;
        Debug.Log("Trying to combine ingredient with essence: " + ingredient.name + " and " + essence.Type);

        if (ingredient == null || essence == null)
        {
            return false;
        }

        // Calculate enhancement percentage based on essence strength
        float enhancementPercentage = essence.GetStrengthLevel() switch
        {
            "Weak" => 0.1f,
            "Moderate" => 0.2f,
            "Strong" => 0.3f,
            "Epic" => 0.5f,
            _ => 0f
        };

        // Create a new enhanced ingredient
        SOIngredient enhancedIngredient = ScriptableObject.CreateInstance<SOIngredient>();
        enhancedIngredient.CopyFrom(ingredient);  // Copy base properties
        enhancedIngredient.IsEnhanced = true;

        // Apply enhancement
        Stats stats = enhancedIngredient.Stats;
        switch (essence.Type)
        {
            case Essence.EssenceType.Alcohol:
                stats.Alcohol += stats.Alcohol * enhancementPercentage;
                break;
            case Essence.EssenceType.Toxicity:
                stats.Toxicity += stats.Toxicity * enhancementPercentage;
                break;
            case Essence.EssenceType.Sweetness:
                stats.Sweetness += stats.Sweetness * enhancementPercentage;
                break;
            case Essence.EssenceType.Bitterness:
                stats.Bitterness += stats.Bitterness * enhancementPercentage;
                break;
            case Essence.EssenceType.Sourness:
                stats.Sourness += stats.Sourness * enhancementPercentage;
                break;
        }
        enhancedIngredient.Stats = stats;

        result = enhancedIngredient;
        Debug.Log("Enhanced ingredient created: " + result.name);
        return true;
    }
}


