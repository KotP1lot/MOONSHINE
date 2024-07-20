using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LootboxSystem : MonoBehaviour
{
    [System.Serializable]
    public class Lootbox
    {
        public string Name;
        public int Cost;
        public List<RarityChance> RarityChances;
    }

    [System.Serializable]
    public class RarityChance
    {
        public string Rarity;
        public float Chance;
    }

    public List<Lootbox> AvailableLootboxes;
    private float _playerMoney = 200000;

    [SerializeField] private IngredientsManager _ingredientsManager;

    private void Start()
    {
        if (_ingredientsManager == null)
        {
            _ingredientsManager = FindObjectOfType<IngredientsManager>();
        }
    }

    public float GetPlayerMoney()
    {
        return _playerMoney;
    }

    public void BuyLootbox(int lootboxIndex)
    {
        if (lootboxIndex < 0 || lootboxIndex >= AvailableLootboxes.Count)
        {
            Debug.LogError("Invalid lootbox index");
            return;
        }

        Lootbox selectedLootbox = AvailableLootboxes[lootboxIndex];

        if (_playerMoney >= selectedLootbox.Cost)
        {
            _playerMoney -= selectedLootbox.Cost;
            OpenLootbox(selectedLootbox);
        }
        else
        {
            Debug.Log("Not enough money to buy this lootbox");
        }
    }

    private void OpenLootbox(Lootbox lootbox)
    {
        string selectedRarity = GetRandomRarity(lootbox.RarityChances);
        SOIngredient receivedIngredient = GetRandomIngredientByRarity(selectedRarity);

        if (receivedIngredient != null)
        {
            if (receivedIngredient.Unlocked)
            {
                float refund = lootbox.Cost / 2f;
                _playerMoney += refund;
                Debug.Log($"Ingredient {receivedIngredient.name} already unlocked. Refunded {refund} coins.");
            }
            else
            {
                _ingredientsManager.UnlockIngredient(receivedIngredient);
                Debug.Log($"Opened lootbox: {lootbox.Name}. Received new ingredient: {receivedIngredient.name} (Rarity: {receivedIngredient.Rarity})");
            }
        }
        else
        {
            Debug.LogWarning($"No ingredient found for Rarity: {selectedRarity}");
        }
    }

    private string GetRandomRarity(List<RarityChance> RarityChances)
    {
        float totalChance = RarityChances.Sum(rc => rc.Chance);
        float randomValue = Random.Range(0f, totalChance);
        float cumulativeChance = 0f;

        foreach (var RarityChance in RarityChances)
        {
            cumulativeChance += RarityChance.Chance;
            if (randomValue <= cumulativeChance)
            {
                return RarityChance.Rarity;
            }
        }

        return RarityChances[RarityChances.Count - 1].Rarity;
    }

    private SOIngredient GetRandomIngredientByRarity(string Rarity)
    {
        List<SOIngredient> allIngredients = _ingredientsManager.GetAllIngredients();
        List<SOIngredient> ingredientsOfRarity = allIngredients.FindAll(i => i.Rarity.ToLower() == Rarity.ToLower());

        if (ingredientsOfRarity.Count > 0)
        {
            return ingredientsOfRarity[Random.Range(0, ingredientsOfRarity.Count)];
        }

        return null;
    }

    public void AddMoney(float amount)
    {
        _playerMoney += amount;
    }
}