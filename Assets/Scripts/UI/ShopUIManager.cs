using UnityEngine;

public class ShopUIManager : MonoBehaviour
{
    public GameObject heartUI; // Reference to the Heart UI
    //public GameObject timerUI; // Reference to the Timer UI
    public GameObject shopPanel; // Reference to the Shop UI panel
    public GameObject breadPrefab; // The bread item to be given when purchased
    public Transform playerTransform; // Where the bread will be given (player position)
    
    public PlayerCurrencyManager currencyManager; // Reference to the PlayerCurrencyManager

    void Start()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false); // Ensure shop starts closed
    }

    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            
            if (heartUI != null)
                heartUI.SetActive(false); // Hide hearts when shop opens

            //if (timerUI != null)
            //    timerUI.SetActive(false); // Hide timer when shop opens
        }
    }

    public void BuyBread()
    {
        if (currencyManager != null && currencyManager.playerCurrency >= 15) // Ensure enough money
        {
            currencyManager.BuyItem(); // Deduct money from currency manager

            // Spawn bread at player's position (optional)
            if (breadPrefab != null && playerTransform != null)
            {
                Instantiate(breadPrefab, playerTransform.position, Quaternion.identity);
            }

            CloseShop(); // Close shop after purchase
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);

            if (heartUI != null)
                heartUI.SetActive(true); // Re-enable hearts when shop closes

            //if (timerUI != null)
            //    timerUI.SetActive(true); // Re-enable timer when shop closes
        }
    }
}
