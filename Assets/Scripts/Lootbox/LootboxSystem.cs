using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LootboxSystem : MonoBehaviour
{
    [System.Serializable]
    public class Lootbox
    {
        public string name;
        public int cost;
        public List<RarityChance> rarityChances;
    }

    [System.Serializable]
    public class RarityChance
    {
        public string rarity;
        public float chance;
    }

    public List<Lootbox> availableLootboxes;
    private float playerMoney = 200000;

    [SerializeField] private IngredientsManager ingredientsManager;

    private void Start()
    {
        if (ingredientsManager == null)
        {
            ingredientsManager = FindObjectOfType<IngredientsManager>();
        }
    }

    public float GetPlayerMoney()
    {
        return playerMoney;
    }

    public void BuyLootbox(int lootboxIndex)
    {
        if (lootboxIndex < 0 || lootboxIndex >= availableLootboxes.Count)
        {
            Debug.LogError("Invalid lootbox index");
            return;
        }

        Lootbox selectedLootbox = availableLootboxes[lootboxIndex];

        if (playerMoney >= selectedLootbox.cost)
        {
            playerMoney -= selectedLootbox.cost;
            OpenLootbox(selectedLootbox);
        }
        else
        {
            Debug.Log("Not enough money to buy this lootbox");
        }
    }

    private void OpenLootbox(Lootbox lootbox)
    {
        string selectedRarity = GetRandomRarity(lootbox.rarityChances);
        SOIngredient receivedIngredient = GetRandomIngredientByRarity(selectedRarity);

        if (receivedIngredient != null)
        {
            if (receivedIngredient.unlocked)
            {
                float refund = lootbox.cost / 2f;
                playerMoney += refund;
                Debug.Log($"Ingredient {receivedIngredient.name} already unlocked. Refunded {refund} coins.");
            }
            else
            {
                ingredientsManager.UnlockIngredient(receivedIngredient);
                Debug.Log($"Opened lootbox: {lootbox.name}. Received new ingredient: {receivedIngredient.name} (Rarity: {receivedIngredient.rarity})");
            }
        }
        else
        {
            Debug.LogWarning($"No ingredient found for rarity: {selectedRarity}");
        }
    }

    private string GetRandomRarity(List<RarityChance> rarityChances)
    {
        float totalChance = rarityChances.Sum(rc => rc.chance);
        float randomValue = Random.Range(0f, totalChance);
        float cumulativeChance = 0f;

        foreach (var rarityChance in rarityChances)
        {
            cumulativeChance += rarityChance.chance;
            if (randomValue <= cumulativeChance)
            {
                return rarityChance.rarity;
            }
        }

        return rarityChances[rarityChances.Count - 1].rarity;
    }

    private SOIngredient GetRandomIngredientByRarity(string rarity)
    {
        List<SOIngredient> allIngredients = ingredientsManager.GetAllIngredients();
        List<SOIngredient> ingredientsOfRarity = allIngredients.FindAll(i => i.rarity.ToLower() == rarity.ToLower());

        if (ingredientsOfRarity.Count > 0)
        {
            return ingredientsOfRarity[Random.Range(0, ingredientsOfRarity.Count)];
        }

        return null;
    }

    public void AddMoney(float amount)
    {
        playerMoney += amount;
    }
}