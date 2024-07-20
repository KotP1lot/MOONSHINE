using System;
using System.Collections;
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
    [SerializeField] private List<SOIngredient> _ingredients;

    [SerializeField] private int _buyingPrice;
    [Space]
    [SerializeField] private int _usesPerClient;
    [SerializeField] private int _maxUsesPerClient;
    [Space]
    [SerializeField] private int _upgradeCost;
    [SerializeField] private int _upgradeCostModifier;

    private bool _isUnlocked = false;

    [SerializeField] private Resource _money;

    public bool TryCombine(SOIngredient ingredient1Index, SOIngredient ingredient2Index, out SOIngredient result)
    {
        result = null;

        if (!_isUnlocked)
            return false;

        foreach (var combination in _availableCombinations)
        {
            if ((combination.Ingredient1 == ingredient1Index && combination.Ingredient2 == ingredient2Index) ||
                (combination.Ingredient1 == ingredient2Index && combination.Ingredient2 == ingredient1Index))
            {
                result = combination.Result;
                return true;
            }
        }

        return false;
    }
    public bool CanUse(int currentUses)
    {
        return _isUnlocked && currentUses < _usesPerClient;
    }

    public bool TryUpgrade()
    {
        if (_usesPerClient < _maxUsesPerClient)
        {
            _money.Spend(_upgradeCost);
            _upgradeCost*=-_upgradeCostModifier;

            _usesPerClient++;
            return true;
        }
        return false;
    }

    public void Unlock()
    {
        _isUnlocked = true;
    }
}
