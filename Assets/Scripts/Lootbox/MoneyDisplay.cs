using UnityEngine;
using TMPro;

public class MoneyDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private LootboxSystem lootboxSystem;

    private void Start()
    {
        if (lootboxSystem == null)
        {
            lootboxSystem = FindObjectOfType<LootboxSystem>();
        }

        if (moneyText == null)
        {
            moneyText = GetComponent<TextMeshProUGUI>();
        }

        UpdateMoneyDisplay();
    }

    private void Update()
    {
        UpdateMoneyDisplay();
    }

    private void UpdateMoneyDisplay()
    {
        if (lootboxSystem != null && moneyText != null)
        {
            moneyText.text = $"Money: {lootboxSystem.GetPlayerMoney():F2}";
        }
    }
}