using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combinator : Aparat
{
    [Serializable]
    private class Combination
    {
        public SOIngredient Ingredient1;
        public SOIngredient Ingredient2;
        public SOIngredient Result;
    }

    [SerializeField] private List<Combination> _availableCombinations;
    [SerializeField] private Button _combineButton;
    [SerializeField] private SlotTrigger _firstSlot;
    [SerializeField] private SlotTrigger _secondSlot;
    [SerializeField] private Transform _spawnPoint;

    private Ingredient _firstIngredient;
    private EssenceComponent _firstEssenceComponent;
    private Ingredient _secondIngredient;
    private EssenceComponent _secondEssenceComponent;

    private void Start()
    {
        _combineButton.onClick.AddListener(OnCombineButtonClick);
    }

    public void OnFirstSlotTriggerStay(Collider other)
    {
        HandleSlotTriggerStay(other, ref _firstIngredient, ref _firstEssenceComponent, "First");
    }

    public void OnSecondSlotTriggerStay(Collider other)
    {
        HandleSlotTriggerStay(other, ref _secondIngredient, ref _secondEssenceComponent, "Second");
    }

    public void OnFirstSlotTriggerExit(Collider other)
    {
        HandleSlotTriggerExit(other, ref _firstIngredient, ref _firstEssenceComponent, "First");
    }

    public void OnSecondSlotTriggerExit(Collider other)
    {
        HandleSlotTriggerExit(other, ref _secondIngredient, ref _secondEssenceComponent, "Second");
    }

    private void HandleSlotTriggerStay(Collider other, ref Ingredient ingredient, ref EssenceComponent essenceComponent, string slotName)
    {
        Debug.Log($"Collider Entered {slotName} Slot: {other.name}");

        Ingredient newIngredient = other.GetComponentInParent<Ingredient>();
        EssenceComponent newEssence = other.GetComponentInParent<EssenceComponent>();

        if (newIngredient != null && ingredient != newIngredient)
        {
            ingredient = newIngredient;
            essenceComponent = null;
            Debug.Log($"{slotName} ingredient detected: {ingredient.Data.name}");
        }
        else if (newEssence != null && essenceComponent != newEssence)
        {
            essenceComponent = newEssence;
            ingredient = null;
            Debug.Log($"{slotName} essence detected: {essenceComponent.GetEssence().Type}");
        }
    }

    private void HandleSlotTriggerExit(Collider other, ref Ingredient ingredient, ref EssenceComponent essenceComponent, string slotName)
    {
        Ingredient exitingIngredient = other.GetComponentInParent<Ingredient>();
        EssenceComponent exitingEssence = other.GetComponentInParent<EssenceComponent>();

        if (exitingIngredient != null && ingredient == exitingIngredient)
        {
            ingredient = null;
            Debug.Log($"{slotName} ingredient removed: {exitingIngredient.Data.name}");
        }
        else if (exitingEssence != null && essenceComponent == exitingEssence)
        {
            essenceComponent = null;
            Debug.Log($"{slotName} essence removed: {exitingEssence.GetEssence().Type}");
        }
    }

    private void OnCombineButtonClick()
    {
        if (_firstIngredient != null && _secondEssenceComponent != null)
        {
            CombineIngredientWithEssence();
        }
        else if (_firstIngredient != null && _secondIngredient != null)
        {
            CombineIngredients();
        }
        else
        {
            Debug.LogError("Invalid combination attempt.");
        }

        ResetSlots();
    }

    private void CombineIngredientWithEssence()
    {
        if (_firstIngredient.Data.IsEnhanced)
        {
            Debug.LogError("First ingredient is already enhanced and cannot be combined.");
            return;
        }

        Debug.Log("Attempting to combine ingredient and essence.");
        Essence essence = _secondEssenceComponent.GetEssence();
        if (TryCombineIngredientWithEssence(_firstIngredient.Data, essence, out SOIngredient result))
        {
            Debug.Log("Combination successful.");
            _firstIngredient.Setup(result);
            _firstIngredient.transform.position = _spawnPoint.position;
            Destroy(_secondEssenceComponent.gameObject);
        }
        else
        {
            Debug.LogError("Combination failed.");
        }
    }

    private void CombineIngredients()
    {
        if (_firstIngredient.Data.IsEnhanced || _secondIngredient.Data.IsEnhanced)
        {
            Debug.LogError("One or both ingredients are already enhanced and cannot be combined.");
            return;
        }

        Debug.Log("Attempting to combine two ingredients.");
        if (TryCombine(_firstIngredient.Data, _secondIngredient.Data, out SOIngredient result))
        {
            Debug.Log("Combination successful.");
            _firstIngredient.Setup(result);
            _firstIngredient.transform.position = _spawnPoint.position;
            Destroy(_secondIngredient.gameObject);
        }
        else
        {
            Debug.LogError("Combination failed.");
        }
    }

    private bool TryCombine(SOIngredient ingredient1, SOIngredient ingredient2, out SOIngredient result)
    {
        result = null;
        Debug.Log($"Trying to combine: {ingredient1.name} and {ingredient2.name}");

        foreach (var combination in _availableCombinations)
        {
            if ((combination.Ingredient1 == ingredient1 && combination.Ingredient2 == ingredient2) ||
                (combination.Ingredient1 == ingredient2 && combination.Ingredient2 == ingredient1))
            {
                result = combination.Result;
                Debug.Log($"Found valid combination: {result.name}");
                return true;
            }
        }

        Debug.LogError("No valid combination found.");
        return false;
    }

    private bool TryCombineIngredientWithEssence(SOIngredient ingredient, Essence essence, out SOIngredient result)
    {
        result = null;
        Debug.Log($"Trying to combine ingredient with essence: {ingredient.name} and {essence.Type}");

        if (ingredient == null || essence == null)
        {
            return false;
        }

        float enhancementPercentage = GetEnhancementPercentage(essence.GetStrengthLevel());

        SOIngredient enhancedIngredient = ScriptableObject.CreateInstance<SOIngredient>();
        enhancedIngredient.CopyFrom(ingredient);
        enhancedIngredient.IsEnhanced = true;

        EnhanceStats(ref enhancedIngredient.Stats, essence.Type, enhancementPercentage);

        result = enhancedIngredient;
        Debug.Log($"Enhanced ingredient created: {result.name}");
        return true;
    }

    private float GetEnhancementPercentage(string strengthLevel) => strengthLevel switch
    {
        "Weak" => 0.1f,
        "Moderate" => 0.2f,
        "Strong" => 0.3f,
        "Epic" => 0.5f,
        _ => 0f
    };

    private void EnhanceStats(ref Stats stats, Essence.EssenceType essenceType, float enhancementPercentage)
    {
        switch (essenceType)
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
    }

    private void ResetSlots()
    {
        _firstIngredient = null;
        _firstEssenceComponent = null;
        _secondIngredient = null;
        _secondEssenceComponent = null;
    }
}