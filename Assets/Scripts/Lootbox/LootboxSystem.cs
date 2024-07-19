using System.Collections.Generic;
using UnityEngine;

public class LootboxSystem : MonoBehaviour
{
    [System.Serializable]
    public class Ingredient
    {
        public string name;
        public Rarity rarity;
    }

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    [System.Serializable]
    public class Lootbox
    {
        public string name;
        public int cost;
        public int ingredientCount;
        public List<float> rarityChances; // Шансы выпадения каждой редкости
    }

    public List<Ingredient> allIngredients;
    public List<Lootbox> availableLootboxes;

    private float playerMoney;

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
        List<Ingredient> receivedIngredients = new List<Ingredient>();

        for (int i = 0; i < lootbox.ingredientCount; i++)
        {
            Rarity selectedRarity = GetRandomRarity(lootbox.rarityChances);
            Ingredient randomIngredient = GetRandomIngredientByRarity(selectedRarity);

            if (randomIngredient != null)
            {
                receivedIngredients.Add(randomIngredient);
            }
        }

        // Здесь можно добавить логику для отображения полученных ингредиентов
        Debug.Log($"Opened lootbox: {lootbox.name}. Received {receivedIngredients.Count} ingredients.");
    }

    private Rarity GetRandomRarity(List<float> rarityChances)
    {
        float randomValue = Random.value;
        float cumulativeChance = 0f;

        for (int i = 0; i < rarityChances.Count; i++)
        {
            cumulativeChance += rarityChances[i];
            if (randomValue <= cumulativeChance)
            {
                return (Rarity)i;
            }
        }

        return Rarity.Common; // Если что-то пошло не так, возвращаем обычную редкость
    }

    private Ingredient GetRandomIngredientByRarity(Rarity rarity)
    {
        List<Ingredient> ingredientsOfRarity = allIngredients.FindAll(i => i.rarity == rarity);

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