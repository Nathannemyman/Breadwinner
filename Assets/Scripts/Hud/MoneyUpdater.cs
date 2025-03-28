using UnityEngine;
using TMPro;

public class MoneyUpdater : MonoBehaviour
{
    [SerializeField]
    private bool set_money_to_zero_on_start = true;

    private TextMeshProUGUI moneyText;

    private void Start()
    {
        // Get the TextMeshProUGUI component on this GameObject
        moneyText = GetComponent<TextMeshProUGUI>();

        if (moneyText == null)
        {
            Debug.LogError("MoneyUpdater requires a TextMeshProUGUI component!", this);
            enabled = false; // Disable this script if no text component found
        }

        // Only set the GameManager money value to 0 if the bool is true
        if (GameManager.Instance != null && set_money_to_zero_on_start)
        {
            GameManager.Instance.Money = 0;
            Debug.LogWarning("SETTING MONEY TO 0 !!!!! 123");
        }
        else if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager instance not found! Unable to set money to 0.", this);
        }
    }

    private void Update()
    {
        // Make sure GameManager instance exists and we have a text component
        if (GameManager.Instance != null && moneyText != null)
        {
            // Update the text with the current money value
            moneyText.text = GameManager.Instance.Money.ToString();
        }
    }
}